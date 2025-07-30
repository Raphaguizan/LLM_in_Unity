using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Guizan.API;
using Guizan.LLM.Utils;

namespace Guizan.LLM
{
    public class GroqLLM : MonoBehaviour
    {
        private const string apiUrl = "https://api.groq.com/openai/v1/chat/completions";

        [SerializeField]
        private APIKeyConfig groqKey;


        private string apiKey;
        private void Awake()
        {
            apiKey = groqKey.Key;
        }

        public void EnviarMensagem(string textoUsuario)
        {
            StartCoroutine(EnviarParaGroq(textoUsuario));
        }

        IEnumerator EnviarParaGroq(string textoUsuario)
        {
            var mensagens = new List<Message>
        {
            new Message { role = "system", content = "System Text" },
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

            if (request.result == UnityWebRequest.Result.Success)
            {
                string respostaJson = request.downloadHandler.text;
                Debug.Log("Resposta da IA: " + respostaJson.ExtractLLMMessage());
            }
            else
            {
                Debug.LogError("Erro ao enviar para a Groq: " + request.error);
            }
        }
    }
}