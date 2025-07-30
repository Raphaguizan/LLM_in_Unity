using UnityEngine;

namespace Guizan.LLM
{
    public class ResponseLLM
    {
        public ResponseType type;
        public string responseText;
    }

    public enum ResponseType
    {
        Success,
        Error
    }
}