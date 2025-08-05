using NaughtyAttributes;
using Newtonsoft.Json;
using System;

namespace Guizan.LLM
{
    [Serializable]
    public class Message 
    {
        [JsonProperty("role")]
        public string role;
        [JsonProperty("content"), ResizableTextArea]
        public string content;

        public Message(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}
