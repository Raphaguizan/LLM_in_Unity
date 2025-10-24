using System.Collections.Generic;

namespace Guizan.LLM.Agent
{
    public interface IAgentAction
    {
        public string Type { get; }
        public void MakeAction(Dictionary<string, object> Parameters);
    }
}