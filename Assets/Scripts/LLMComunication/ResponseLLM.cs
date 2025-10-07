using UnityEngine;

namespace Guizan.LLM
{
    public class ResponseLLM
    {
        public ResponseType type;
        public string responseText;

        public override string ToString()
        {
            return "Type: "+type+"\nText: "+responseText;
        }
    }

    public enum ResponseType
    {
        Success,
        Error
    }
}