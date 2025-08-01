using Guizan.LLM;
using System.Collections.Generic;
using UnityEngine;

public class TestEmbedding : MonoBehaviour
{
    [SerializeField] private CohereEmbeddingClient client;

    void Start()
    {
        List<string> textos = new List<string> {
            "O céu está azul.",
            "A IA pode ajudar em jogos."
        };

        client.RequestEmbeddings(textos);
        client.EmbedResponseEvent.AddListener(ReceiveResponse);
    }

    private void ReceiveResponse(CohereEmbeddingResponse response)
    {
        Debug.Log(response.responseType);
        Debug.Log(response.embeddings);
        Debug.Log(string.Join(",", response.embeddings[0]));
        client.EmbedResponseEvent.RemoveListener(ReceiveResponse);
    }
}
