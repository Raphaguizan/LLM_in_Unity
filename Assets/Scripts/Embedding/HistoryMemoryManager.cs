using Guizan.LLM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Guizan.LLM.Embedding
{
    [RequireComponent(typeof(LLMAgent))]
    public class HistoryMemoryManager : MonoBehaviour
    {
        [SerializeField, Range(-1f, 1f)]
        private float similarityAccept = .5f;
        
        [SerializeField]
        private AgentEmbedding embeddings;

        private LLMAgent agent;
        void Start()
        {
            agent = GetComponent<LLMAgent>();
            AgentHistoryMemory.LoadAgentMemory(agent.AgentConfigs.AgentID, ReceiveLoadResponse);
        }

        private void ReceiveLoadResponse(AgentEmbedding response)
        {
            //Debug.Log(response.Embeddings);
            //Debug.Log(string.Join(",", response.Embeddings[0]));
            embeddings = response;
            //client.EmbedResponseEvent.RemoveListener(ReceiveResponse);
        }

        public void MakeEmbedding(string text, Action conclusion = null)
        {
            CohereEmbeddingClient.RequestEmbeddings(text, (emb, type) => {
                if(type == ResponseType.Success)
                {
                    var (index, score) = EmbeddingUtils.GetMostSimilarEmbedding(emb.Embeddings[0], embeddings);
                    Debug.Log($"score : {score}  Index : {index}\n\ntexto:\n{text}");
                    if(score > similarityAccept && !ChuckAlreadySent(index))
                    {
                        SendSystemEmbeddingMessage(index, conclusion);
                        return;
                    }
                }
                else
                {
                    Debug.LogError("Error ao receber embedding\n\n"+emb);
                }
                conclusion?.Invoke();
            });
        }
        private bool ChuckAlreadySent(int chunkIndex)
        {
            return agent.AgentConfigs.Messages.Any(msg => msg.role == "system" && msg.content.Contains(embeddings.TextChunks[chunkIndex]));
        }

        private void SendSystemEmbeddingMessage(int chunkIndex, Action conclusion = null)
        {
            //GroqLLM.SendMessageToLLM(agent.AgentConfigs, MakeSystemPrompt(chunkIndex), (x) => conclusion?.Invoke());
        }


        private Message MakeSystemPrompt(int chunkIndex)
        {
            string chunkText = embeddings.TextChunks[chunkIndex];
            string prompt = $"Agora leve em consideração na hora de responder ao usuário essa história do personagem que você está interpertando:\n\"{chunkText}\"";
            return new Message(MessageRole.system, prompt);
        }
    }
}