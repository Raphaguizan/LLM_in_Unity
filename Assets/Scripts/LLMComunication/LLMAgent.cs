using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using Guizan.LLM.Embedding;
using System;


namespace Guizan.LLM
{
    public class LLMAgent : MonoBehaviour
    {
        [SerializeField, Tooltip("Frase que será enviada caso ocorra algum erro. Ex: request não retornar Success.")]
        private string defaultMessage = "Error";
        [SerializeField, Expandable]
        private AgentConfigs myConfigs;
        [Space]
        public UnityEvent<string> AnswerEvent;

        public AgentConfigs AgentConfigs => myConfigs;

        public void ReceiveText(string message)
        {
            EmbeddingTest(message, () => 
            {
                GroqLLM.SendMessageToLLM(myConfigs, new("user", message));
                GroqLLM.ResponseEvent.AddListener(ReceiveAndSendLLMAnswer);
            });
        }

        private void ReceiveAndSendLLMAnswer(ResponseLLM response)
        {
            if (response == null || response.AgentID != myConfigs.AgentID)
                return;

            if(response.type == ResponseType.Success)
            {
                myConfigs.AddMessage(new("assistant",response.responseText));
                AnswerEvent.Invoke(response.responseText);
            }
            else
            {
                Debug.LogError(response.responseText);
                AnswerEvent.Invoke(defaultMessage);
            }

            GroqLLM.ResponseEvent.RemoveListener(ReceiveAndSendLLMAnswer);
        }

        private void EmbeddingTest(string playerMessage, Action embeddingTested)
        {
            if (!TryGetComponent<HistoryMemoryManager>(out var hmm))
            {
                embeddingTested?.Invoke();
                Debug.Log("sem componente HistoryMemoryManager");
                return;
            }

            hmm.MakeEmbedding(playerMessage, embeddingTested);
        }
    }
}