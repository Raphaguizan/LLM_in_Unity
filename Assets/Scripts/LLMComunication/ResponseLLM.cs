using UnityEngine;

namespace Guizan.LLM
{
    public class ResponseLLM
    {
        public ResponseType type;
        public ResponseFunction function = ResponseFunction.User;
        public string responseText;
    }

    public enum ResponseType
    {
        Success,
        Error
    }

    public enum ResponseFunction
    {
        System,
        User
    }
}