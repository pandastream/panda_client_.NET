using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace Panda.Core
{
    public class ServiceRequest : IServiceRequest
    {
        /// <summary>
        /// The base url path of the panda service
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The HTTP request verb
        /// </summary>
        public string Verb { get; set; }

        /// <summary>
        /// The signed parameters passed to the Panda service. Please ensure that all required parameters
        /// (i.e. access_key, signature...) are supplied.
        /// </summary>
        public string SignedParameters { get; set; }

        /// <summary>
        /// The name of the file to upload to the Panda service
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The content (in bytes) of the file to upload to the Panda service
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// A flag indicating whether or not a file will be posted to the panda service
        /// </summary>
        public bool IsFilePosted
        {
            get
            {
                return File != null && !string.IsNullOrEmpty(FileName);
            }
        }

        /// <summary>
        /// Will validate the supplied data to ensure that all required data for a panda
        /// service call was supplied.
        /// </summary>
        /// <param name="brokenRulesMessage">Messages associated with any and all broken rules</param>
        /// <returns>A flag indicating whther or not the request is valid for submission</returns>
        public bool IsValidForSubmission(out string brokenRulesMessage)
        {
            brokenRulesMessage = string.Empty;

            if (string.IsNullOrEmpty(BaseUrl))
                brokenRulesMessage += "A BaseUrl is required to submit a panda service request. ";
            if (string.IsNullOrEmpty(Verb))
                brokenRulesMessage += "A Verb is required to submit a panda service request. ";
            if (string.IsNullOrEmpty(SignedParameters))
                brokenRulesMessage += "SignedParameters are required to submit a panda service request. ";

            // if any file related data is supplied, ensure that all required file-related data exists
            if (File != null || !string.IsNullOrEmpty(FileName))
            {
                if(File == null || string.IsNullOrEmpty(FileName))
                    brokenRulesMessage += "Both File and FileName are required to submit a file to a panda service request. ";
            }

            return string.IsNullOrEmpty(brokenRulesMessage);
        }

        /// <summary>
        /// A flag indicating whther or not the request submits associated data (such as an HTTP POST or PUT)
        /// </summary>
        public bool HasDataToSend
        {
            get { return (Verb == "POST" || Verb == "PUT"); }
        }

        /// <summary>
        /// The full url of the Panda service
        /// </summary>
        public string Url
        {
            get 
            {
                // if request does not send data, append signed parameters to the url
                return (this.HasDataToSend) ? BaseUrl : string.Format("{0}?{1}", BaseUrl, SignedParameters);
            }
        }

        private NameValueCollection _parameters;
        private NameValueCollection Parameters
        {
            get
            {
                if(_parameters == null)
                    _parameters = System.Web.HttpUtility.ParseQueryString(SignedParameters);
                return _parameters;
            }
        }

        /// <summary>
        /// Creates the HTTP request that will be sent to the Panda service.
        /// </summary>
        /// <returns>Web request</returns>
        public WebRequest CreateWebRequest()
        {
            string brokenRulesMessage;
            if (!this.IsValidForSubmission(out brokenRulesMessage))
                throw new ArgumentException(brokenRulesMessage);

            if (this.IsFilePosted)
                return CreateMultipartFormRequest();
            else
                return CreateFormUrlEncodedRequest();
        }

        private WebRequest CreateFormUrlEncodedRequest()
        {
            var request = WebRequest.Create(new Uri(Url));
            request.Method = Verb;

            // if request must data to send, write the data to the request's content stream
            if (this.HasDataToSend)
            {
                var byteArray = Encoding.UTF8.GetBytes(SignedParameters);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                var requestStream = request.GetRequestStream();
                requestStream.Write(byteArray, 0, byteArray.Length);
                requestStream.Close();
            }

            return request;
        }

        private WebRequest CreateMultipartFormRequest()
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            // instantiate the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            System.IO.Stream requestStream = request.GetRequestStream();

            // write each of the supplied parameters to the request's stream
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in Parameters.Keys)
            {
                requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, Parameters[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                requestStream.Write(formitembytes, 0, formitembytes.Length);
            }
            requestStream.Write(boundarybytes, 0, boundarybytes.Length);

            // write the file contents and meta to the request's stream
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "file", FileName, "application/octet-stream");
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            requestStream.Write(headerbytes, 0, headerbytes.Length);
            requestStream.Write(File, 0, File.Length);

            // close and return request
            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            requestStream.Write(trailer, 0, trailer.Length);
            requestStream.Close();
            return request;
        }

        /// <summary>
        /// Submits a request to the Panda service
        /// </summary>
        /// <returns>The HttpWebResponse from the Panda service</returns>
        public HttpWebResponse Send()
        {
            var request = CreateWebRequest();
            var response = (HttpWebResponse)request.GetResponse();
            return response;
        }
    }
}
