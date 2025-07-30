using Newtonsoft.Json.Linq;

namespace Guizan.LLM.Utils
{
    public static class LLMUtils
    {
        public static string ExtractLLMMessage(this string json)
        {
            JObject obj = JObject.Parse(json);
            return (string)obj["choices"]?[0]?["message"]?["content"];
        }
    }
}