using System;
using System.Net;
namespace Panda.Core
{
    public interface IServiceRequest
    {
        string BaseUrl { get; set; }
        string Verb { get; set; }
        string SignedParameters { get; set; }
        string FileName { get; set; }
        byte[] File { get; set; }
        IWebProxy Proxy { get; set; }

        bool IsFilePosted { get; }
        bool IsValidForSubmission(out string brokenRulesMessage);
        bool HasDataToSend { get; }
        string Url { get; }

        System.Net.WebRequest CreateWebRequest();
        System.Net.HttpWebResponse Send();        
    }
}