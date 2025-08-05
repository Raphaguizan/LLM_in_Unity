using Guizan.API;
using Guizan.LLM.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Game.Util;
using System;

namespace Guizan.LLM
{
    public class GroqLLM : Singleton<GroqLLM>
    {
        private const string apiUrl = "https://api.groq.com/openai/v1/chat/completions";

        [SerializeField]
        private APIKeyConfig groqKey;

        [Space, SerializeField]
        public static UnityEvent<ResponseLLM> ResponseEvent;

        private string apiKey;


        protected override void Awake()
        {
            base.Awake();
            apiKey = groqKey.Key;
            ResponseEvent = new();
        }

        public static void SendMessageToLLM(AgentConfigs agent, Message userMessage, Action<ResponseLLM> onResponse = null)
        {
            agent.AddMessage(userMessage);
            Instance.StartCoroutine(Instance.SendToGroq(agent,onResponse));
        }

        IEnumerator SendToGroq(AgentConfigs agent, Action<ResponseLLM> onResponse = null)
        {
            string jsonPayload = JsonUtility.ToJson(agent.GetRequest());

            // Corrige campos faltando do JsonUtility (como null e arrays)
            jsonPayload = jsonPayload.Replace("\"stop\":\"\"", "\"stop\":null");

            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            ResponseLLM response = new()
            {
                AgentID = agent.AgentID
            };

            if (request.result == UnityWebRequest.Result.Success)
            {
                string answerJson = request.downloadHandler.text;
                response.type = ResponseType.Success;
                response.responseText = answerJson.ExtractLLMMessage();
            }
            else
            {
                //Debug.LogError("Erro ao enviar para a Groq: " + request.error);

                response.type = ResponseType.Error;
                response.responseText = request.error;
            }

            //Debug.Log(response);
            ResponseEvent.Invoke(response);
            onResponse?.Invoke(response);
        }
    }
}