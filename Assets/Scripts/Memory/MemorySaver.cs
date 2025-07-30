using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Guizan.LLM.Memory 
{
    public class MemorySaver
    {
        private static string MemoryFolder => Path.Combine(Application.persistentDataPath, "npc_memory");


        /// <summary>
        /// Carrega histórico de conversa do NPC.
        /// </summary>
        public static List<Message> LoadMemory(string npcId, string playerId = "default")
        {
            string path = GetMemoryPath(npcId, playerId);
            if (!File.Exists(path)) return new List<Message>();

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Message>>(json) ?? new List<Message>();
        }

        /// <summary>
        /// Salva histórico de conversa do NPC.
        /// </summary>
        public static void SaveMemory(string npcId, List<Message> messages, string playerId = "default")
        {
            string path = GetMemoryPath(npcId, playerId);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            string json = JsonConvert.SerializeObject(messages, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void ClearMemory(string npcId, string playerId = "default")
        {
            string path = GetMemoryPath(npcId, playerId);
            if (File.Exists(path))
                File.Delete(path);
        }

        private static string GetMemoryPath(string npcId, string playerId)
        {
            string fileName = $"{npcId}_{playerId}.json";
            return Path.Combine(MemoryFolder, fileName);
        }
    }
}