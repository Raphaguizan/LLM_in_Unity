using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class InventoryInjector : TalkInjector
    {
        [SerializeField]
        private string testText = "inventory injection";
        public override string GetTextToInject()
        {
            return testText;
        }
    }
}