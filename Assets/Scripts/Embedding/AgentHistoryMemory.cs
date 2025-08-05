using System;
using System.Collections.Generic;
using UnityEngine;
using Guizan.LLM.Utils;


namespace Guizan.LLM.Embedding
{
    public static class AgentHistoryMemory
    {
        public static void LoadAgentMemory(string agentID, Action<AgentEmbedding>  response)
        {
            AgentEmbedding agentEmbedding = EmbeddingSaveManager.LoadMemory(agentID);
            if(agentEmbedding.Embeddings != null)
            {
                Debug.Log("Carregando embedding da História salva.");
                response.Invoke(agentEmbedding);
                return;
            }

            string story = LoadNPCStory(agentID);
            if (story == null)
            {
                Debug.LogError("Error! história não encontrada!");
                return;
            }
            List<string> storyChuncks = story.SplitTextIntoChunksWithOverlap(256, 32);


            Debug.Log("Gerando novo embedding da História.");
            CohereEmbeddingClient.RequestEmbeddings(storyChuncks);
            CohereEmbeddingClient.EmbedResponseEvent.AddListener((embedding, type) => ReceiveEmbeddinResponse(embedding, type, agentID, response));
        }

        private static void ReceiveEmbeddinResponse(AgentEmbedding embedding, ResponseType type, string agentID, Action<AgentEmbedding> response)
        {
            if(type == ResponseType.Error)
            {
                Debug.LogError("Error! ao receber os novos embeddins");
                return;
            }

            SaveNPCStory(agentID, embedding);
            response.Invoke(embedding);
        }

        private static void SaveNPCStory(string agentID, AgentEmbedding embeddins)
        {
            EmbeddingSaveManager.SaveMemory(agentID, embeddins);
        }
        private static string LoadNPCStory(string agentID)
        {
            string fileName = agentID+"_History";
            TextAsset textAsset = Resources.Load<TextAsset>($"NPCsHistories/{fileName}");
            if (textAsset == null)
            {
                //Debug.LogError($"Erro: não foi possível carregar {fileName} de Resources/NPCsHistories.");
                return null;
            }

            return textAsset.text;
        }
    }
}