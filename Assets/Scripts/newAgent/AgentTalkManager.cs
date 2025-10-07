using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Guizan.LLM.Agent
{

    public class AgentTalkManager : MonoBehaviour
    {
        [SerializeField, Expandable]
        private AgentConfigs configs;
        private AgentMemoryManager memory;

        private void Awake()
        {
            memory = GetComponent<AgentMemoryManager>();
        }
        public void SendMessage(string message, MessageRole role = MessageRole.user)
        {
            Message newMessage = new(role, message);
            List<Message> messages = new(){newMessage};

            if (memory != null)
            {
                memory.AddMemory(newMessage);
                messages = memory.Memory;
            }

            GroqLLM.SendMessageToLLM(messages, configs, ReceiveAnswer);
        }

        private void ReceiveAnswer(ResponseLLM response)
        {
            Debug.Log(response.responseText);

            if(memory != null)
            {
                memory.AddMemory(MessageRole.assistant, response.responseText);
            }
        }
    }
}