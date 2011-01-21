using System;
namespace Panda.Core
{
    public interface IServiceProxyUtility
    {
        string EncodeStringToHMACSHA256(string stringToSign, string secretKey);
        string GetPandaTimestamp(DateTime time);
        string GetPandaTimestamp();
    }
}
