using Guizan.LLM;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Embedding
{
    [RequireComponent(typeof(LLMAgent))]
    public class HistoryMemoryManager : MonoBehaviour
    {
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
    }
}