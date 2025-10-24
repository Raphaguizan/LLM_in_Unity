using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class InventoryInjector : TalkInjector
    {
        public override string GetTextToInject()
        {
            return "inventory injector";
        }
    }
}