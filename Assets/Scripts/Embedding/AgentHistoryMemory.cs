using System;
using System.Collections.Generic;
using UnityEngine;
using Guizan.LLM.Utils;


namespace Guizan.LLM.Embedding
{
    public static class AgentHistoryMemory
    {
        public static void LoadAgentMemory(string agentID, Action<AgentEmbedding>  onResponse)
        {
            AgentEmbedding agentEmbedding = AgentJSONSaver<AgentEmbedding>.LoadJSON(agentID, SavePathFolder.embedding);

            string story = LoadNPCStory(agentID);
            List<string> storyChunks = MakeChunks(story);

            if(agentEmbedding != default && CompareChunks(agentEmbedding.TextChunks, storyChunks))
            {
                Debug.Log("Carregando embedding da História salva.");
                onResponse?.Invoke(agentEmbedding);
                return;
            }

            if (story == null)
            {
                Debug.LogError("Error! história não encontrada!");
                return;
            }

            Debug.Log("Gerando novo embedding da História.");
            CohereEmbeddingClient.RequestEmbeddings(storyChunks);
            CohereEmbeddingClient.EmbedResponseEvent.AddListener((embedding, type) => ReceiveEmbeddinResponse(embedding, type, agentID, onResponse));
        }

        private static void ReceiveEmbeddinResponse(AgentEmbedding embedding, ResponseType type, string agentID, Action<AgentEmbedding> onResponse)
        {
            if(type == ResponseType.Error)
            {
                Debug.LogError("Error! ao receber os novos embeddins");
                return;
            }

            SaveNPCStory(agentID, embedding);
            onResponse?.Invoke(embedding);
        }

        private static void SaveNPCStory(string agentID, AgentEmbedding embeddins)
        {
            AgentJSONSaver<AgentEmbedding>.SaveJSON(agentID, embeddins, SavePathFolder.embedding);
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

        public static List<string> MakeChunks(string text)
        {
            return text.SplitTextIntoChunksWithOverlap(256, 32);
        }

        private static bool CompareChunks(List<string> chunk1, List<string> chunk2)
        {
            if (chunk1 == null || chunk2 == null)
                return false;

            for (int i = 0; i < chunk1.Count; i++)
            {
                if (!chunk1[i].Equals(chunk2[i]))
                    return false;
            }
            return true;
        }
        public static void ClearMemory(string agentID)
        {
            AgentJSONSaver<AgentEmbedding>.ClearJSON(agentID);
        }
    }
}