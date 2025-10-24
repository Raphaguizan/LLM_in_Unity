using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class PlayerAppearanceInjector : TalkInjector
    {
        public override string GetTextToInject()
        {
            return "player appearance";
        }
    }
}