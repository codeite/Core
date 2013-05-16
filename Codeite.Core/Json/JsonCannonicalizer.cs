namespace Codeite.Core.Json
{
    public class JsonCannonicalizer
    {
        public string Cannonicalize(string json)
        {
            return DynamicJsonObject.ToCannonicalString(DynamicJsonObject.ReadJson(json));
        }
    }
}
