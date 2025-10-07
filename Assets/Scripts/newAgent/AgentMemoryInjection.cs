using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    [RequireComponent(typeof(AgentMemoryManager))]
    public class AgentMemoryInjection : MonoBehaviour
    {
        [SerializeField]
        private List<Message> injectionsList;

        AgentMemoryManager memoryManager;

        void Awake()
        {
            memoryManager = GetComponent<AgentMemoryManager>();
            memoryManager.MemoryResetCallBack.AddListener(OnMemoryCallBack);
        }

        private void OnMemoryCallBack()
        {
            InjectMemory();
        }

        [Button]
        public void InjectMemory()
        {
            for (int i = 0; i < injectionsList.Count; i++)
            {
                memoryManager.AddMemory(injectionsList[i], i);
            }
        }

        private void Reset()
        {
            memoryManager = GetComponent<AgentMemoryManager>();
            InjectMemory();
        }

        private void OnDestroy()
        {
            memoryManager.MemoryResetCallBack.RemoveAllListeners();
        }
    }
}