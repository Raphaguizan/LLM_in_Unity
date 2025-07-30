using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Guizan.API;
using Guizan.LLM.Utils;
using UnityEngine.Events;

namespace Guizan.LLM
{
    public class GroqLLM : MonoBehaviour
    {
        private const string apiUrl = "https://api.groq.com/openai/v1/chat/completions";

        [SerializeField]
        private APIKeyConfig groqKey;

        [Space, SerializeField]
        public UnityEvent<ResponseLLM> ResponseEvent;


        private string apiKey;
        private void Awake()
        {
            apiKey = groqKey.Key;
        }

        public void SendMessageToLLM(string textoUsuario)
        {
            StartCoroutine(SendToGroq(textoUsuario));
        }

        IEnumerator SendToGroq(string textoUsuario)
        {
            var mensagens = new List<Message>
        {
            new Message { role = "system", content = "Você é um assistente que responde de forma clara e direta, usando apenas texto. Não use emojis nem emoticons em nenhuma resposta." },
            new Message { role = "user", content = textoUsuario }
        };

            var requisicao = new ChatRequest
            {
                messages = mensagens,
                model = "gemma2-9b-it",
                temperature = 1f,
                max_completion_tokens = 1024,
                top_p = 1f,
                stream = false,
                stop = null // ou deixe como "" se necessário
            };

            string jsonPayload = JsonUtility.ToJson(requisicao);

            // Corrige campos faltando do JsonUtility (como null e arrays)
            jsonPayload = jsonPayload.Replace("\"stop\":\"\"", "\"stop\":null");

            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            ResponseLLM response = new();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string respostaJson = request.downloadHandler.text;
                //Debug.Log("Resposta da IA: " + respostaJson.ExtractLLMMessage());

                response.type = ResponseType.Success;
                response.responseText = respostaJson.ExtractLLMMessage();
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