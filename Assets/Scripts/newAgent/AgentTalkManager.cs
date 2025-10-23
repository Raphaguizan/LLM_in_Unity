using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using Guizan.LLM.Embedding;

namespace Guizan.LLM.Agent
{

    public class AgentTalkManager : MonoBehaviour
    {
        [SerializeField, ResizableTextArea, Tooltip("Frase que será enviada caso ocorra algum erroe o request não retornar Success.")]
        private string defaultMessage = "Error";
        private AgentMemoryManager memory;
        private AgentEmbeddingManager embedding;

        private Action<string> responseCallBack;

        public string DefaultMessage => defaultMessage;
        private void Awake()
        {
            embedding = GetComponent<AgentEmbeddingManager>();
            memory = GetComponent<AgentMemoryManager>();
            responseCallBack = null;
        }
        public void SendMessage(string message, MessageRole role = MessageRole.user, Action<string> callback = null)
        {
            Message newMessage = new(role, message);
            List<Message> messages = new(){newMessage};

            responseCallBack = callback;

            if (memory != null)
            {
                memory.AddMemory(newMessage);
                messages = memory.Memory;
            }

            if (embedding != null)
            {
                embedding.TestEmbedding(message, () => { GroqLLM.SendMessageToLLM(messages, ReceiveAnswer);});
                return;
            }

            GroqLLM.SendMessageToLLM(messages, ReceiveAnswer);
        }

        private void ReceiveAnswer(ResponseLLM response)
        {
            if(response.type == ResponseType.Error)
            {
                Debug.LogError(defaultMessage);  
                response.responseText = defaultMessage;
            } 
            else if (memory != null)
            {
                memory.AddMemory(MessageRole.assistant, response.responseText);
            }

            responseCallBack?.Invoke(response.responseText);
            responseCallBack = null;
        }
    }
}