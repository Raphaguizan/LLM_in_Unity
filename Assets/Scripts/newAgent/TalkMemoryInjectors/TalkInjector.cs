using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class TalkInjector : MonoBehaviour
    {
        [SerializeField]
        private AgentTalkMemoryInjection injectManager;

        protected virtual void Awake()
        {
            injectManager.Subscribe(this);
        }

        public virtual string GetTextToInject()
        {
            return "";
        }
    }
}