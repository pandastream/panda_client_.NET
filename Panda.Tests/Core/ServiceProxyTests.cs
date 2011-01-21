using System;
using System.Collections.Generic;
using NUnit.Framework;
using Panda.Core;
using Rhino.Mocks;

namespace Panda.Tests.Core
{
    [TestFixture]
    public class ServiceProxyTests
    {
        private readonly string _cloudId = "some_cloud_id";
        private readonly string _accessKey = "some_access_key";
        private readonly string _secretKey = "some_secret_key";
        private readonly string _apiHost = "api.pandastream.com";
        private readonly int _apiPort = 100;
        private readonly int _apiVersion = 4;
        private readonly IServiceProxyUtility _utility = MockRepository.GenerateStub<IServiceProxyUtility>();
        private readonly IServiceRequest _serviceRequest = MockRepository.GenerateStub<IServiceRequest>();

        private ServiceProxy GetDefaultProxy()
        {
            return new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost, _utility, _serviceRequest);
        }

        private ServiceProxy GetCustomProxy()
        {
            return new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost, _apiPort, _apiVersion, _utility, _serviceRequest);
        }

        [Test]
        public void ApiUrl_DefaultPortAndVersionUsed_ReturnsFullApiUrl()
        {
            var proxy = GetDefaultProxy();

            Assert.AreEqual("http://api.pandastream.com/v2", proxy.ApiUrl, "should return API url defaulting to port 80 and version 2");
        }

        [Test]
        public void ApiUrl_SuppliedPortAndVersionUsed_ReturnsFullApiUrl()
        {
            var proxy = GetCustomProxy();

            Assert.AreEqual("http://api.pandastream.com:100/v4", proxy.ApiUrl, "should return API url containing port 100 and version 4 as was supplied in the constructor.");
        }

        [Test]
        public void Get_ValidGETRequestDataSupplied_SendRequestSendMethodIsCalled()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost, new ServiceProxyUtility(), _serviceRequest);

            proxy.Get("videos.json", new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            _serviceRequest.AssertWasCalled(req => req.Send());
        }

        [Test]
        public void Post_ValidPOSTRequestDataSupplied_SendRequestSendMethodIsCalled()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost, new ServiceProxyUtility(), _serviceRequest);

            proxy.Post("videos.json", new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            _serviceRequest.AssertWasCalled(req => req.Send());
        }

        [Test]
        public void Post_FileDataSupplied_ServiceRequestPopulatedWithFileData()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost, new ServiceProxyUtility(), _serviceRequest);
            byte[] file = new byte[8];
            string fileName = "somefile.jpg";

            proxy.Post("videos.json", new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                }, file, fileName);

            Assert.AreEqual(file, _serviceRequest.File);
            Assert.AreEqual(fileName, _serviceRequest.FileName);
        }

        [Test]
        public void Delete_ValidParametersSupplied_SendRequestSendMethodIsCalled()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost, new ServiceProxyUtility(), _serviceRequest);

            proxy.Delete("videos.json", new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            _serviceRequest.AssertWasCalled(req => req.Send());
        }

        [Test]
        public void StringToSign_ValidRequestDataSupplied_ReturnsValidStringToSign()
        {
            var proxy = GetDefaultProxy();

            var stringToSign = proxy.StringToSign("get", "videos.json", "api.pandastream.com", 
                new Dictionary<string,string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            Assert.AreEqual("GET\napi.pandastream.com\nvideos.json\nanother_parameter=another_value&some_parameter=some_value", 
                stringToSign, 
                "should return a string with the verb, host, request path and alphabetically sorted parameters separated by new lines.");
        }

        [Test]
        public void SignatureGenerator_ValidRequestDataSupplied_UtilityEncodeMethodCalled()
        {
            var proxy = GetDefaultProxy();
            var generatedStringToSign = "GET\napi.pandastream.com\nvideos.json\nanother_parameter=another_value&some_parameter=some_value";

            var signature = proxy.SignatureGenerator("get", "videos.json", "api.pandastream.com", _secretKey, 
                new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            _utility.AssertWasCalled(u => u.EncodeStringToHMACSHA256(generatedStringToSign, _secretKey));
        }

        [Test]
        public void SignatureGenerator_ValidRequestDataSupplied_ReturnsValidSignature()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost);
            
            var signature = proxy.SignatureGenerator("get", "videos.json", "api.pandastream.com", _secretKey,
                new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            Assert.AreEqual("z4gJjCHfeJtPjhFKcepIvmfYSyRQPTPbkTGpRoo0UWU=", signature);
        }

        [Test]
        public void BuildRequest_GETRequestDataSupplied_ReturnsValidIPandaServiceRequest()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost);

            var request = proxy.BuildRequest("get", "videos.json", new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            Assert.IsTrue(request.Verb == "GET", "request method should retain verb passed to RestClient");
            Assert.IsTrue(request.BaseUrl == "http://api.pandastream.com/v2/videos.json", "base url should contain the host, version and path passed to RestClient");
            Assert.IsTrue(request.Url.Contains("http://api.pandastream.com/v2/videos.json?some_parameter=some_value&another_parameter=another_value&access_key=some_access_key&cloud_id=some_cloud_id&timestamp="),
                "Url should contain host, version, path and query string parameters for GET requests");
            Assert.IsTrue(request.SignedParameters.Contains(
                "some_parameter=some_value&another_parameter=another_value&access_key=some_access_key&cloud_id=some_cloud_id&timestamp="),
                "SignedParameters should contain the supplied parameters, access key, cloudId and timestamp in the query string of a GET request");
            Assert.IsTrue(request.SignedParameters.Contains(
                "&signature="),
                "SignedParameters should be signed");

            Console.WriteLine("Verb: {0}", request.Verb);
            Console.WriteLine("BaseUrl: {0}", request.BaseUrl);
            Console.WriteLine("SignedParameters: {0}", request.SignedParameters);
            Console.WriteLine("Url: {0}", request.Url);
        }

        [Test]
        public void BuildRequest_POSTRequestDataSupplied_ReturnsValidIPandaServiceRequest()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost);

            var request = proxy.BuildRequest("post", "videos.json", new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            Assert.IsTrue(request.Verb == "POST", "request method should retain verb passed to RestClient");
            Assert.IsTrue(request.BaseUrl == "http://api.pandastream.com/v2/videos.json", "base url should contain the host, version and path passed to RestClient");
            Assert.IsTrue(request.Url.Equals("http://api.pandastream.com/v2/videos.json"),
                "Url should contain host, version and path with no query string parameters for POST and PUT requests");

            Console.WriteLine("Verb: {0}", request.Verb);
            Console.WriteLine("BaseUrl: {0}", request.BaseUrl);
            Console.WriteLine("SignedParameters: {0}", request.SignedParameters);
            Console.WriteLine("Url: {0}", request.Url);
        }

        [Test]
        public void BuildRequest_PUTRequestDataSupplied_ReturnsValidIPandaServiceRequest()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost);

            var request = proxy.BuildRequest("put", "videos.json", new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            Assert.IsTrue(request.Verb == "PUT", "request method should retain verb passed to RestClient");
            Assert.IsTrue(request.BaseUrl == "http://api.pandastream.com/v2/videos.json", "base url should contain the host, version and path passed to RestClient");
            Assert.IsTrue(request.Url.Equals("http://api.pandastream.com/v2/videos.json"),
                "Url should contain host, version and path with no query string parameters for POST and PUT requests");

            Console.WriteLine("Verb: {0}", request.Verb);
            Console.WriteLine("BaseUrl: {0}", request.BaseUrl);
            Console.WriteLine("SignedParameters: {0}", request.SignedParameters);
            Console.WriteLine("Url: {0}", request.Url);
        }

        [Test]
        public void SendRequest_ValidRequestDataSupplied_SendRequestIsCalled()
        {
            var proxy = new ServiceProxy(_cloudId, _accessKey, _secretKey, _apiHost, new ServiceProxyUtility(), _serviceRequest);
            
            proxy.SendRequest("get", "videos.json", new Dictionary<string, string>
                {
                    { "some_parameter", "some_value" },
                    { "another_parameter", "another_value" }
                });

            _serviceRequest.AssertWasCalled(req => req.Send());
        }
    }
}
