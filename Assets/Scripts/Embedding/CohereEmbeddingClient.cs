using Guizan.API;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Game.Util;
using NaughtyAttributes;

namespace Guizan.LLM.Embedding
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

    [Serializable]
    public class AgentEmbedding
    {
        [JsonProperty("embeddings"), ShowNativeProperty]
        public List<List<float>> Embeddings { get; set; }

        public AgentEmbedding() 
        {
            Embeddings = null;
        }

        public AgentEmbedding(List<List<float>> embeddings)
        {
            this.Embeddings = embeddings;
        }
    }

    public class CohereEmbeddingClient : Singleton<CohereEmbeddingClient>
    {
        [Header("Cohere API Settings")]
        [SerializeField] private APIKeyConfig apiKey;
        private const string endpoint = "https://api.cohere.ai/v1/embed";

        [Space]
        public static UnityEvent<AgentEmbedding, ResponseType> EmbedResponseEvent;

        protected override void Awake()
        {
            base.Awake();
            EmbedResponseEvent = new();
        }

        public static void RequestEmbeddings(List<string> texts)
        {
            Instance.StartCoroutine(Instance.RequestEmbeddingsCoroutine(texts));
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

            AgentEmbedding response = new();
            ResponseType type = ResponseType.Success;

            if (request.result == UnityWebRequest.Result.Success)
            {
                // A resposta é um JSON com "embeddings": [[...], [...]]
                string responseText = request.downloadHandler.text;
                response = JsonConvert.DeserializeObject<AgentEmbedding>(responseText);
            }
            else
            {
                type = ResponseType.Error;
                response.Embeddings = null;
            }

            EmbedResponseEvent.Invoke(response, type);
        }
    }
}
