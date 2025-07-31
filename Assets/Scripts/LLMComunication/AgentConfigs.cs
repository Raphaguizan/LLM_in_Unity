using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Guizan.LLM
{
    [CreateAssetMenu(fileName = "NewAgentConfig", menuName = "LLM/AgentConfig")]
    public class AgentConfigs : ScriptableObject
    {
        [SerializeField]
        private string agentID;

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


        [Space]
        public UnityEvent<string> MessagesChangeEvent;

        public string AgentID => agentID;
        public List<Message> Messages => messages;
        public int Lenght => Messages.Count;

        private void Awake()
        {
            MessagesChangeEvent = new();
        }

        public void LoadMessages(List<Message> messages)
        {
            this.messages = messages;
            this.MessagesChangeEvent.Invoke(messages[Lenght - 1].role);
        }

        public void ResetMessages()
        {
            this.messages = new()
            {
                new("system", "Voc� � um assistente que responde de forma clara e direta, usando apenas texto. N�o use emojis nem emoticons em nenhuma resposta.")
            };
            this.MessagesChangeEvent.Invoke("system");
        }

        public void AddMessage(Message message)
        {
            this.messages.Add(message);
            this.MessagesChangeEvent.Invoke(message.role);
        }

        public AgentRequest GetRequest()
        {
            return new AgentRequest(messages, model, temperature, Max_completion_tokens, Top_p, Stream, Stop);
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