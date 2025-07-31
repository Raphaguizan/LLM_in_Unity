using UnityEngine;

namespace Guizan.LLM
{
    public class ResponseLLM
    {
        public ResponseType type;
        public string AgentID;
        public string responseText;

        public override string ToString()
        {
            return "Type: "+type+"  AgentID: "+AgentID+"\nText: "+responseText;
        }
    }

    public enum ResponseType
    {
        Success,
        Error
    }
}