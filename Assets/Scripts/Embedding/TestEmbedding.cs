using Guizan.LLM.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Embedding
{
    public class TestEmbedding : MonoBehaviour
    {
        [SerializeField] private CohereEmbeddingClient client;

        void Start()
        {
            string texto = NPCMemoryLoader.LoadNPCStory("NPC1TesteHistory");
            List<string> textos = texto.SplitTextIntoChunksWithOverlap(256, 32);

            Debug.Log(texto); 
            Debug.Log(textos);
            Debug.Log(string.Join("<--\n\n-->", textos));
            //client.RequestEmbeddings(textos);
            //client.EmbedResponseEvent.AddListener(ReceiveResponse);
        }

        private void ReceiveResponse(CohereEmbeddingResponse response)
        {
            Debug.Log(response.responseType);
            Debug.Log(response.embeddings);
            Debug.Log(string.Join(",", response.embeddings[0]));
            client.EmbedResponseEvent.RemoveListener(ReceiveResponse);
        }
    }
}