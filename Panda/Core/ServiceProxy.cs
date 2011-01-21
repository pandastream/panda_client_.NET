using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Panda.Core
{
    /// <summary>
    /// Service Proxy for Panda Video API 
    /// </summary>
    public class ServiceProxy : IServiceProxy
    {
        #region Private Members
        private readonly IServiceProxyUtility _utility;
        private readonly IServiceRequest _serviceRequest;
        private readonly string _cloudId;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _apiHost;
        private readonly int _apiPort = 80;
        private readonly int _apiVersion = 2; 
        #endregion

        #region Ctors
        public ServiceProxy(string cloudId, string accessKey, string secretKey, string apiHost)
        {
            _cloudId = cloudId;
            _apiHost = apiHost;
            _secretKey = secretKey;
            _accessKey = accessKey;
            _utility = new ServiceProxyUtility();
            _serviceRequest = new ServiceRequest();
        }

        public ServiceProxy(string cloudId, string accessKey, string secretKey, string apiHost, int apiPort, int apiVersion) :
            this(cloudId, accessKey, secretKey, apiHost)
        {
            _apiPort = apiPort;
            _apiVersion = apiVersion;
        }

        // additional constructors created to support unit testing
        public ServiceProxy(string cloudId, string accessKey, string secretKey, string apiHost, IServiceProxyUtility utility, IServiceRequest serviceRequest) :
            this(cloudId, accessKey, secretKey, apiHost)
        {
            _utility = utility;
            _serviceRequest = serviceRequest;
        }

        public ServiceProxy(string cloudId, string accessKey, string secretKey, string apiHost, int apiPort, int apiVersion, IServiceProxyUtility utility, IServiceRequest serviceRequest) :
            this(cloudId, accessKey, secretKey, apiHost, apiPort, apiVersion)
        {
            _utility = utility;
            _serviceRequest = serviceRequest;
        } 
        #endregion

        #region Properties
        /// <summary>
        /// API Url and port (port number appended only if it is not port 80)
        /// </summary>
        private string ApiHostAndPort
        {
            get
            {
                return (_apiPort == 80) ? _apiHost : string.Format("{0}:{1}", _apiHost, _apiPort);
            }
        }

        /// <summary>
        /// API base path
        /// </summary>
        private string ApiBasePath
        {
            get { return "/v" + _apiVersion; }
        }

        /// <summary>
        /// Full API Url including protocol, host, port and base path
        /// </summary>
        public string ApiUrl
        {
            get { return string.Format("http://{0}{1}", ApiHostAndPort, ApiBasePath); }
        } 
        #endregion

        #region Request Methods

        public HttpWebResponse Get(string path, Dictionary<string, string> parameters)
        {
            return SendRequest("GET", path, parameters);
        }

        public HttpWebResponse Post(string path, Dictionary<string, string> parameters)
        {
            return Post(path, parameters, null, null);
        }

        public HttpWebResponse Post(string path, Dictionary<string, string> parameters,
            byte[] file, string fileName)
        {
            return SendRequest("POST", path, parameters, file, fileName);
        }

        public HttpWebResponse Put(string path, Dictionary<string, string> parameters)
        {
            return SendRequest("PUT", path, parameters);
        }

        public HttpWebResponse Delete(string path, Dictionary<string, string> parameters)
        {
            return SendRequest("DELETE", path, parameters);
        } 

        #endregion

        #region JSON Request Methods

        public string GetJson(string path, Dictionary<string, string> parameters)
        {
            try
            {
                var response = Get(path, parameters);
                return GetResponseString(response);
            }
            catch (WebException ex)
            {
                LogDebugInfo("Web Exception", ex.Message);
                var handler = OnWebException;
                if (handler != null) handler(ex);
            }
            return string.Empty;
        }

        public string PutJson(string path, Dictionary<string, string> parameters)
        {
            try
            {
                var response = Put(path, parameters);
                return GetResponseString(response);
            }
            catch (WebException ex)
            {
                LogDebugInfo("Web Exception", ex.Message);
                var handler = OnWebException;
                if (handler != null) handler(ex);
            }
            return string.Empty;
        }

        public string PostJson(string path, Dictionary<string, string> parameters)
        {
            return PostJson(path, parameters, null, null);
        }

        public string PostJson(string path, Dictionary<string, string> parameters,
            byte[] file, string fileName)
        {
            try
            {
                var response = Post(path, parameters, file, fileName);
                return GetResponseString(response);
            }
            catch (WebException ex)
            {
                LogDebugInfo("Web Exception", ex.Message);
                var handler = OnWebException;
                if (handler != null) handler(ex);
            }
            return string.Empty;
        }

        public string DeleteJson(string path, Dictionary<string, string> parameters)
        {
            try
            {
                var response = Delete(path, parameters);
                return GetResponseString(response);
            }
            catch (WebException ex)
            {
                LogDebugInfo("Web Exception", ex.Message);
                var handler = OnWebException;
                if (handler != null) handler(ex);
            }
            return string.Empty;
        }


        private static string GetResponseString(WebResponse response)
        {
            var stIn = new StreamReader(response.GetResponseStream());
            return stIn.ReadToEnd();
        }

        #endregion

        #region Request Construction Methods

        /// <summary>
        /// Creates the HTTP request that will be sent to the Panda service.
        /// </summary>
        /// <param name="verb">HTTP request verb</param>
        /// <param name="path">HTTP request path</param>
        /// <param name="parameters">Parameters passed to the Panda service</param>
        /// <returns>Web request</returns>
        public IServiceRequest BuildRequest(string verb, string path, Dictionary<string, string> parameters)
        {
            verb = verb.ToUpper();
            path = CanonicalPath(path);
            var timeStamp = DateTime.UtcNow;

            _serviceRequest.BaseUrl = ApiUrl + path;
            _serviceRequest.Verb = verb;
            _serviceRequest.SignedParameters = SignedQuery(verb, path, parameters, timeStamp);

            return _serviceRequest;
        }

        /// <summary>
        /// Generates a signature used for Panda service authentication. The request verb, host, 
        /// path and additional parameters are HMACSHA256 encoded using the supplied secret key
        /// </summary>
        /// <param name="verb">HTTP request verb</param>
        /// <param name="requestPath">HTTP request path</param>
        /// <param name="host">HTTP request host</param>
        /// <param name="secretKey">The secret key supplied by Panda when an account is first created</param>
        /// <param name="parameters">Parameters passed to the Panda service</param>
        /// <returns>The generated signature</returns>
        public string SignatureGenerator(string verb, string requestPath, string host, string secretKey, Dictionary<string, string> parameters)
        {
            var stringToSign = StringToSign(verb, requestPath, host, parameters);
            return _utility.EncodeStringToHMACSHA256(stringToSign, secretKey);
        }

        /// <summary>
        /// Generates the string value used to create the signature passed to the Panda services for 
        /// authentication.
        /// </summary>
        /// <param name="verb">HTTP request verb</param>
        /// <param name="requestPath">HTTP request path</param>
        /// <param name="host">HTTP request host</param>
        /// <param name="parameters">Parameters passed to the Panda service</param>
        /// <returns>The string used to generate the authetication signature</returns>
        public string StringToSign(string verb, string requestPath, string host, Dictionary<string, string> parameters)
        {
            // sort parameters alaphabetically and remove the 'file' parameter if supplied
            var sortedParameters = from item in parameters
                                   where item.Key != "file"
                                   orderby item.Key ascending
                                   select new KeyValuePair<string, string>(item.Key, item.Value);

            // encode parameters
            var querystring = EncodeToQuery(sortedParameters.ToDictionary(x => x.Key, x => x.Value));

            // build the string used to create the signature
            var stringToSign = string.Format("{0}\n{1}\n{2}\n{3}", verb.ToUpper(), host.ToLower(),
                requestPath, querystring);

            LogDebugInfo("StringToSign", stringToSign);
            return stringToSign;
        }

        public HttpWebResponse SendRequest(string verb, string path, Dictionary<string, string> parameters)
        {
            return SendRequest(verb, path, parameters, null, null);
        }

        public HttpWebResponse SendRequest(string verb, string path, Dictionary<string, string> parameters,
            byte[] file, string fileName)
        {
            var request = BuildRequest(verb, path, parameters);
            request.File = file;
            request.FileName = fileName;
            var response = request.Send();
            return response;
        }

        private string GenerateSignature(string verb, string requestPath, Dictionary<string, string> parameters)
        {
            var signature = SignatureGenerator(verb, requestPath, _apiHost, _secretKey, parameters);
            LogDebugInfo("Signature", signature);
            return signature;
        }

        private string SignedQuery(string verb, string requestPath, Dictionary<string, string> parameters, DateTime timeStamp)
        {
            var signedQuery = EncodeToQuery(SignedParams(verb, requestPath, parameters, timeStamp));
            LogDebugInfo("signedQuery", signedQuery);
            return signedQuery;
        }

        private Dictionary<string, string> SignedParams(string verb, string requestPath, Dictionary<string, string> parameters, DateTime timeStamp)
        {
            var authparams = new Dictionary<string, string>(parameters)
                                 {
                                     {"access_key", _accessKey},
                                     {"cloud_id", _cloudId},
                                     {"timestamp", _utility.GetPandaTimestamp(timeStamp)}
                                 };

            LogDebugInfo("timestamp", authparams["timestamp"]);
            authparams.Add("signature", GenerateSignature(verb, requestPath, authparams));

            return authparams;
        }

        private static string CanonicalPath(string path)
        {
            return "/" + path.Trim(' ', '\t', '\n', '\r', '\0');
        }

        private static string EncodeToQuery(Dictionary<string, string> parameters)
        {
            var items = new Collection<string>();
            if (parameters != null)
                foreach (var keyValuePair in parameters)
                {
                    if (keyValuePair.Key == "timestamp")
                    {
                        items.Add(keyValuePair.Key + "=" + HttpUtility.UrlEncode(keyValuePair.Value).ToUpper());
                        continue;
                    }
                    if (keyValuePair.Key == "file")
                    {
                        items.Add(keyValuePair.Key + "=" + keyValuePair.Value);
                        continue;
                    }
                    items.Add(keyValuePair.Key + "=" + UpperCaseUrlEncode(keyValuePair.Value));
                }
            return string.Join("&", items.ToArray());
        }

        private static string UpperCaseUrlEncode(string s)
        {
            char[] temp = HttpUtility.UrlEncode(s).ToCharArray();
            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
            return new string(temp);
        }

        #endregion

        #region Debugging Events

        public delegate void DebugEventHandler(string title, string message);
        public event DebugEventHandler OnDebugEvent;

        public delegate void WebExceptionHandler(WebException exception);
        public event WebExceptionHandler OnWebException;

        private void LogDebugInfo(string title, string info)
        {
            var handler = OnDebugEvent;
            if (handler != null) handler(title, info);
        } 

        #endregion
    }
}