using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Guizan.LLM.Embedding
{
    public class EmbeddingSaveManager
    {
        private static string EmbeddingFolder => Path.Combine(Application.persistentDataPath, "npc_Embedding");


        /// <summary>
        /// Carrega histórico de conversa do NPC.
        /// </summary>
        public static AgentEmbedding LoadMemory(string npcId)
        {
            string path = GetMemoryPath(npcId);
            if (!File.Exists(path)) return new();

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<AgentEmbedding>(json) ?? new();
        }

        /// <summary>
        /// Salva histórico de conversa do NPC.
        /// </summary>
        public static void SaveMemory(string npcId, AgentEmbedding embeddings)
        {
            string path = GetMemoryPath(npcId);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            string json = JsonConvert.SerializeObject(embeddings, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void ClearMemory(string npcId)
        {
            string path = GetMemoryPath(npcId);
            if (File.Exists(path))
                File.Delete(path);
        }

        private static string GetMemoryPath(string npcId)
        {
            string fileName = $"{npcId}.json";
            return Path.Combine(EmbeddingFolder, fileName);
        }
    }
}
