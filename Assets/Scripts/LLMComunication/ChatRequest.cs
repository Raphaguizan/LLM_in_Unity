using System;
using System.Collections.Generic;

namespace Guizan.LLM
{
    [Serializable]
    public class ChatRequest
    {
        public List<Message> messages;
        public string model;
        public float temperature;
        public int max_completion_tokens;
        public float top_p;
        public bool stream;
        public string stop;
    }
}