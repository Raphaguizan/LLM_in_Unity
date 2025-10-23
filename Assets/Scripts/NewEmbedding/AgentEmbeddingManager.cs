using Guizan.LLM.Agent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Embedding
{
    public class AgentEmbeddingManager : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        private float similarityAccept = .5f;

        [SerializeField]
        AgentMemoryManager memoryManager = null;

        [SerializeField]
        private List<FileEmbedding> FileEmbeddings;

        public void TestEmbedding(string textToTest, Action onEmbeddingResponse = null)
        {
            try
            {
                MakeEmbedding(textToTest, (emb) =>
                {
                    foreach (var embedding in FileEmbeddings)
                    {
                        TestFileEmbeddingSimilarity(emb, embedding);
                    }
                    onEmbeddingResponse();
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                onEmbeddingResponse();
            }
        }

        private void MakeEmbedding(string text, Action<AgentEmbedding> onCohereResponse)
        {
            CohereEmbeddingClient.RequestEmbeddings(text, (emb, type) => {
                AgentEmbedding resp = default;

                if (type == ResponseType.Success)
                    resp = emb;
                else
                    Debug.LogError("Erro na resposta do cohere: " + emb.ToString() + "  " + type.ToString());

                onCohereResponse.Invoke(resp);
            });
        }

        private void TestFileEmbeddingSimilarity(AgentEmbedding agentEmb, FileEmbedding fileEmb)
        {
            try
            {
                fileEmb.GetEmbeddings((embeddings) =>
                {
                    var (index, score) = EmbeddingUtils.GetMostSimilarEmbedding(agentEmb.Embeddings[0], embeddings);
                    Debug.Log($"score : {score}  Index : {index}\n\ntexto:\n{agentEmb.TextChunks[0]}");
                    if (score > similarityAccept)
                    {
                        MakeSystemPrompt(fileEmb, index, AddEmbeddingMemory);
                    }
                });
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
                throw new Exception(e.Message);
            }

        }

        private void MakeSystemPrompt(FileEmbedding fileEmb, int chunkIndex, Action<Message> messageResponse)
        {
            fileEmb.GetEmbeddings((emb) => {
                string chunkText = emb.TextChunks[chunkIndex];
                string prompt = $"Agora leve em consideração na hora de responder ao usuário essa história do personagem que você está interpertando:\n\"{chunkText}\"";
                messageResponse.Invoke(new Message(MessageRole.system, prompt));
            });
            
        }

        private void AddEmbeddingMemory(Message message)
        {
            if (memoryManager == null)
                return;

            memoryManager.AddMemory(message);
        }
    }
}