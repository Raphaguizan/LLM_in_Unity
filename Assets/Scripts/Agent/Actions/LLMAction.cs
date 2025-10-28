using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent.Actions
{
    /// <summary>
    /// Representa uma a��o que o NPC deve executar.
    /// </summary>
    [System.Serializable]
    public class LLMAction
    {
        /// <summary>
        /// Tipo da a��o, exemplo:
        /// "none", "end_conversation", "follow_player", "give_item"
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Par�metros din�micos (ex: {"item": "potion", "quantity": 2})
        /// </summary>
        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; }

        public LLMAction()
        {
            Type = "none";
            Parameters = new Dictionary<string, object>();
        }
    }
}
