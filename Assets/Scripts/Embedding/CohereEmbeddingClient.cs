using Guizan.API;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Guizan.LLM
{
    [Serializable]
    public class CohereEmbeddingRequest
    {
        public List<string> texts;
        public string model = "embed-multilingual-v3.0";
        public string input_type = "search_document";

        public CohereEmbeddingRequest(List<string> texts)
        {
            this.texts = texts;
        }
    }

    public class CohereEmbeddingResponse
    {
        public ResponseType responseType;
        [JsonProperty("embeddings")]
        public List<List<float>> embeddings { get; set; }
    }

    public class CohereEmbeddingClient : MonoBehaviour
    {
        [Header("Cohere API Settings")]
        [SerializeField] private APIKeyConfig apiKey;
        private const string endpoint = "https://api.cohere.ai/v1/embed";

        [Space]
        public UnityEvent<CohereEmbeddingResponse> EmbedResponseEvent;

        private void Awake()
        {
            EmbedResponseEvent = new();
        }

        public void RequestEmbeddings(List<string> texts)
        {
            StartCoroutine(RequestEmbeddingsCoroutine(texts));
        }
        private IEnumerator RequestEmbeddingsCoroutine(List<string> texts)
        {
            CohereEmbeddingRequest payload = new CohereEmbeddingRequest(texts);
            string json = JsonUtility.ToJson(payload);

            using UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey.Key}");

            yield return request.SendWebRequest();

            CohereEmbeddingResponse response = new();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // A resposta é um JSON com "embeddings": [[...], [...]]
                string responseText = request.downloadHandler.text;
                response = JsonConvert.DeserializeObject<CohereEmbeddingResponse>(responseText);
            }
            else
            {
                response.responseType = ResponseType.Error;
                response.embeddings = null;
            }

            EmbedResponseEvent.Invoke(response);
        }
    }
}
