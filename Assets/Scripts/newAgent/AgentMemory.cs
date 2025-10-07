using Guizan.LLM;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    [CreateAssetMenu(fileName ="newAgentMemory", menuName ="LLM/Memory")]
    public class AgentMemory : ScriptableObject
    {
        [SerializeField]
        private List<Message> memory;

        public List<Message> Memory => memory;

        public void AddMemory(Message newMessage)
        {
            memory.Add(newMessage);
        }

        public void AddMemory(Message newMessage, int index)
        {
            memory.Insert(index, newMessage);
        }

        public void ResetMemories()
        {
            memory.Clear();
        }
    }
}