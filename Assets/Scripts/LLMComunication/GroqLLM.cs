using Guizan.API;
using Guizan.LLM.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Guizan.LLM
{
    public class GroqLLM : MonoBehaviour
    {
        private const string apiUrl = "https://api.groq.com/openai/v1/chat/completions";

        [SerializeField]
        private APIKeyConfig groqKey;

        [Space, SerializeField]
        public UnityEvent<ResponseLLM> ResponseEvent;

        [Space(10f)]
        [SerializeField, Expandable]
        private ChatRequest requestConfig;

        private string apiKey;

        public ChatRequest RequestCofig => requestConfig;
        private void Awake()
        {
            apiKey = groqKey.Key;
        }

        public void SendMessageToLLM(string textoUsuario, ResponseFunction function = ResponseFunction.User)
        {
            StartCoroutine(SendToGroq(textoUsuario, function));
        }

        IEnumerator SendToGroq(string textoUsuario, ResponseFunction function = ResponseFunction.User)
        {
            requestConfig.AddMessage(new("user", textoUsuario));

            var requestConfigs = requestConfig;

            string jsonPayload = JsonUtility.ToJson(requestConfigs);

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
                function = function
            };

            if (request.result == UnityWebRequest.Result.Success)
            {
                string answerJson = request.downloadHandler.text;
                //Debug.Log("Resposta da IA: " + respostaJson.ExtractLLMMessage());

                response.type = ResponseType.Success;
                response.responseText = answerJson.ExtractLLMMessage();
                requestConfig.AddMessage(new("assistant", response.responseText));
            }
            else
            {
                //Debug.LogError("Erro ao enviar para a Groq: " + request.error);

                response.type = ResponseType.Error;
                response.responseText = request.error;
            }

            ResponseEvent.Invoke(response);
        }
    }
}