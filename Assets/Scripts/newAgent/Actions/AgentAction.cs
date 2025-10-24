using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class AgentAction : MonoBehaviour, IAgentAction
    {
        [SerializeField]
        protected string myType;

        public string Type => myType;

        public virtual void MakeAction(Dictionary<string, object> Parameters)
        {
            
        }
    }
}