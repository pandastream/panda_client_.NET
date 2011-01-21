using System;
namespace Panda.Core
{
    public interface IJsonSerializer
    {
        T ConvertToType<T>(object obj);
        T Deserialize<T>(string input);
        object DeserializeObject(string input);
        int MaxJsonLength { get; set; }
        int RecursionLimit { get; set; }
        void Serialize(object obj, System.Text.StringBuilder output);
        string Serialize(object obj);
    }
}
