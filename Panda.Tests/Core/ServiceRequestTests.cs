using NUnit.Framework;
using Panda.Core;

namespace Panda.Tests.Core
{
    [TestFixture]
    public class ServiceRequestTests
    {
        private IServiceRequest GetValidRequest()
        {
            return new ServiceRequest
            {
                BaseUrl = "http://api.pandastream.com/v2/videos.json",
                SignedParameters = "some_parameter=some_value&another_parameter=another_value&access_key=some_access_key&cloud_id=some_cloud_id&timestamp=2011-01-07T15%3A27%3A14%2B00%3A00&signature=zCiK08bThSX%2Bkm0G7383s40EPZwZyjOQCjzlcfdkomM%3D",
                Verb = "GET"
            };
        }

        [Test]
        public void IsFilePosted_RequiredDataSupplied_ReturnsTrue()
        {
            var request = GetValidRequest();
            request.File = new byte[8];
            request.FileName = "some file";
            
            Assert.IsTrue(request.IsFilePosted);
        }

        [Test]
        public void IsFilePosted_RequiredDataMissing_ReturnsFalse()
        {
            var request = GetValidRequest();
            request.File = new byte[8];
            request.FileName = string.Empty;

            Assert.IsFalse(request.IsFilePosted);
        }

        [Test]
        public void IsValidForSubmission_RequiredDataSupplied_ReturnsTrue()
        {
            var request = GetValidRequest();

            string brokenRulesMessage;
            Assert.IsTrue(request.IsValidForSubmission(out brokenRulesMessage));
        }

        [Test]
        public void IsValidForSubmission_NoDataSupplied_ReturnsFalse()
        {
            var request = new ServiceRequest();

            string brokenRulesMessage;
            Assert.IsFalse(request.IsValidForSubmission(out brokenRulesMessage));
        }

        [Test]
        public void IsValidForSubmission_NoVerbSupplied_ReturnsCorrectMessage()
        {
            var request = GetValidRequest();
            request.Verb = string.Empty;

            string brokenRulesMessage;
            Assert.IsFalse(request.IsValidForSubmission(out brokenRulesMessage));
            Assert.AreEqual("A Verb is required to submit a panda service request. ", brokenRulesMessage);
        }

        [Test]
        public void IsValidForSubmission_NoBaseUrlSupplied_ReturnsCorrectMessage()
        {
            var request = GetValidRequest();
            request.BaseUrl = string.Empty;

            string brokenRulesMessage;
            Assert.IsFalse(request.IsValidForSubmission(out brokenRulesMessage));
            Assert.AreEqual("A BaseUrl is required to submit a panda service request. ", brokenRulesMessage);
        }

        [Test]
        public void IsValidForSubmission_NoSignedParametersSupplied_ReturnsCorrectMessage()
        {
            var request = GetValidRequest();
            request.SignedParameters = string.Empty;

            string brokenRulesMessage;
            Assert.IsFalse(request.IsValidForSubmission(out brokenRulesMessage));
            Assert.AreEqual("SignedParameters are required to submit a panda service request. ", brokenRulesMessage);
        }

        [Test]
        public void HasDataToSend_GETRequestDefined_ReturnsFalse()
        {
            var request = GetValidRequest();
            request.Verb = "GET";

            Assert.IsFalse(request.HasDataToSend, "GET requests do not send data. Parameters are appended to the query string");
        }

        [Test]
        public void HasDataToSend_POSTRequestDefined_ReturnsTrue()
        {
            var request = GetValidRequest();
            request.Verb = "POST";

            Assert.IsTrue(request.HasDataToSend, "POST requests send data and do not parameters appended to the query string");
        }

        [Test]
        public void HasDataToSend_PUTRequestDefined_ReturnsTrue()
        {
            var request = GetValidRequest();
            request.Verb = "PUT";

            Assert.IsTrue(request.HasDataToSend, "PUT requests send data and do not parameters appended to the query string");
        }

        [Test]
        public void Url_GETRequestDefined_ConcatenatesParametersToTheUrl()
        {
            var request = GetValidRequest();
            request.Verb = "GET";

            Assert.AreEqual("http://api.pandastream.com/v2/videos.json?some_parameter=some_value&another_parameter=another_value&access_key=some_access_key&cloud_id=some_cloud_id&timestamp=2011-01-07T15%3A27%3A14%2B00%3A00&signature=zCiK08bThSX%2Bkm0G7383s40EPZwZyjOQCjzlcfdkomM%3D",
            request.Url, "GET requests append Parameters to the query string");
        }

        [Test]
        public void Url_POSTRequestDefined_ReturnUrlWithNoAppendedParameters()
        {
            var request = GetValidRequest();
            request.Verb = "POST";

            Assert.AreEqual("http://api.pandastream.com/v2/videos.json",
            request.Url, "POST requests do not append parameters to the query string");
        }

        [Test]
        public void Url_PUTRequestDefined_ReturnUrlWithNoAppendedParameters()
        {
            var request = GetValidRequest();
            request.Verb = "PUT";

            Assert.AreEqual("http://api.pandastream.com/v2/videos.json",
            request.Url, "PUT requests do not append parameters to the query string");
        }
    }
}
