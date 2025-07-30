using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Guizan.LLM
{
    [Serializable]
    [CreateAssetMenu(fileName = "ResquestConfigs", menuName = "LLM/RequestConfig")]
    public class ChatRequest : ScriptableObject
    {
        [SerializeField]
        private List<Message> messages;
        [SerializeField]
        private string model = "gemma2-9b-it";
        [Range(0f, 2f)]
        [SerializeField]
        private float temperature = 1f;
        [SerializeField]
        private int max_completion_tokens = 1024;
        [SerializeField]
        private float top_p = 1f;
        [SerializeField]
        private bool stream = false;
        [SerializeField]
        private string stop = null;


        public List<Message> Messages => messages;
        public int Lenght => Messages.Count;

        public void LoadMessages(List<Message> messages)
        {
            this.messages = messages;
        }

        public void ResetMessages()
        {
            this.messages = new()
            {
                new("system", "Você é um assistente que responde de forma clara e direta, usando apenas texto. Não use emojis nem emoticons em nenhuma resposta.")
            };
        }

        public void AddMessage(Message message)
        {
            this.messages.Add(message);
        }

        public string Model => model;
        public float Temperature => temperature;
        public int Max_completion_tokens => max_completion_tokens;
        public float Top_p => top_p;
        public bool Stream => stream;
        public string Stop => stop;


        private void Reset()
        {
            ResetMessages();
        }
    }
}