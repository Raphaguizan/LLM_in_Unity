using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Guizan.LLM.Agent
{

    public class AgentTalkManager : MonoBehaviour
    {
        [SerializeField, ResizableTextArea, Tooltip("Frase que será enviada caso ocorra algum erroe o request não retornar Success.")]
        private string defaultMessage = "Error";
        private AgentMemoryManager memory;


        public string DefaultMessage => defaultMessage;
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

            GroqLLM.SendMessageToLLM(messages, ReceiveAnswer);
        }

        private void ReceiveAnswer(ResponseLLM response)
        {
            if(response.type == ResponseType.Error)
            {
                Debug.LogError(defaultMessage);
                return;
            }

            if(memory != null)
            {
                memory.AddMemory(MessageRole.assistant, response.responseText);
            }
        }
    }
}