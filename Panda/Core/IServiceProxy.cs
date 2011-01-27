using System;
namespace Panda.Core
{
    public interface IServiceProxy
    {
        string ApiUrl { get; }
        IServiceRequest BuildRequest(string verb, string path, System.Collections.Generic.Dictionary<string, string> parameters);
        System.Net.HttpWebResponse Delete(string path, System.Collections.Generic.Dictionary<string, string> parameters);
        string DeleteJson(string path, System.Collections.Generic.Dictionary<string, string> parameters);
        System.Net.HttpWebResponse Get(string path, System.Collections.Generic.Dictionary<string, string> parameters);
        string GetJson(string path, System.Collections.Generic.Dictionary<string, string> parameters);
        event ServiceProxy.DebugEventHandler OnDebugEvent;
        event ServiceProxy.WebExceptionHandler OnWebException;
        System.Net.HttpWebResponse Post(string path, System.Collections.Generic.Dictionary<string, string> parameters);
        System.Net.HttpWebResponse Post(string path, System.Collections.Generic.Dictionary<string, string> parameters, byte[] file, string fileName);
        string PostJson(string path, System.Collections.Generic.Dictionary<string, string> parameters);
        string PostJson(string path, System.Collections.Generic.Dictionary<string, string> parameters, byte[] file, string fileName);
        System.Net.HttpWebResponse Put(string path, System.Collections.Generic.Dictionary<string, string> parameters);
        string PutJson(string path, System.Collections.Generic.Dictionary<string, string> parameters);
        System.Net.HttpWebResponse SendRequest(string verb, string path, System.Collections.Generic.Dictionary<string, string> parameters);
        IServiceRequest ServiceRequest { get; }
        void SetWebProxyCredentials(string webProxyHost, int webProxyPort, string username, string password);
        string SignatureGenerator(string verb, string requestPath, string host, string secretKey, System.Collections.Generic.Dictionary<string, string> parameters);
        string StringToSign(string verb, string requestPath, string host, System.Collections.Generic.Dictionary<string, string> parameters);
    }
}