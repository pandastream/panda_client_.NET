using System;
using NUnit.Framework;
using Panda.Core;

namespace Panda.Tests.Core
{
    [TestFixture]
    public class ServiceProxyUtilityTests
    {
        [Test]
        public void EncodeStringToHMACSHA256_ValidStringAndKeySupplied_ReturnsEncodedData()
        {
            var stringToEncode = @"GET
api.pandastream.com
/videos.json
access_key=abc123def456&cloud_id=abc123def456&timestamp=2011-01-06T16%3A59%3A44%2B00%3A00";
            var secretKey = "my_secret_key";

            var encodedString = new ServiceProxyUtility().EncodeStringToHMACSHA256(stringToEncode, secretKey);

            Assert.AreEqual("KWRqi3KunCWa7poIshvKkoDRsE8/R+/chat26OrJAyY=", encodedString, "should HMACSHA256 encode provided string using supplied key");
        }

        [Test]
        public void GetPandaTimestamp_ValidDateTimeSupplied_EncodesDateTime()
        {
            var currentDateTime = DateTime.Now;

            var formattedDateTime = new ServiceProxyUtility().GetPandaTimestamp(currentDateTime);

            Assert.AreEqual(currentDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00"),
                formattedDateTime, "should return ISO 8601 formatted DateTime");
        }
    }
}
