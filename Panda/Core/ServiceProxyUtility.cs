using System;
using System.Security.Cryptography;
using System.Text;

namespace Panda.Core
{
    public class ServiceProxyUtility : IServiceProxyUtility
    {
        /// <summary>
        /// Encodes a string into a hash using HMACSHA256
        /// </summary>
        /// <param name="stringToSign"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public string EncodeStringToHMACSHA256(string stringToSign, string secretKey)
        {
            var encoding = new ASCIIEncoding();

            var keyByte = encoding.GetBytes(secretKey);
            var hmacsha256 = new HMACSHA256(keyByte);
            var messageBytes = encoding.GetBytes(stringToSign);
            var hashmessage = hmacsha256.ComputeHash(messageBytes);
            var signature = Convert.ToBase64String(hashmessage);

            return signature;
        }

        /// <summary>
        /// Gets the current time formatted as required for Panda Video signing
        /// </summary>
        /// <returns></returns>
        public string GetPandaTimestamp()
        {
            return GetPandaTimestamp(DateTime.Now);
        }

        /// <summary>
        /// Returns the supplied time formatted as required for Panda Video signing
        /// </summary>
        /// <returns></returns>
        public string GetPandaTimestamp(DateTime time)
        {
            return time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00");
        }
    }
}
