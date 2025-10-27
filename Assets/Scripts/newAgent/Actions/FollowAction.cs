using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class FollowAction : AgentAction
    {
        private void Reset()
        {
            _myType = "follow_player";
        }
        
        [SerializeField]
        private string frase = "Minha frase de teste";
        public override void MakeAction(Dictionary<string, object> Parameters)
        {
            Debug.Log(frase);
        }
    }
}