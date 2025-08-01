using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;


namespace Guizan.LLM
{
    public class LLMAgent : MonoBehaviour
    {
        [SerializeField, Tooltip("Frase que ser� enviada caso ocorra algum erro. Ex: request n�o retornar Success.")]
        private string defaultMessage = "Error";
        [SerializeField, Expandable]
        private AgentConfigs myConfigs;
        [Space]
        public UnityEvent<string> AnswerEvent;

        public AgentConfigs AgentConfigs => myConfigs;

        public void ReceiveText(string message)
        {
            GroqLLM.SendMessageToLLM(myConfigs, new("user", message));
            GroqLLM.ResponseEvent.AddListener(ReceiveAndSendLLMAnswer);
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
    }
}