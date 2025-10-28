using Guizan.LLM.Agent;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent.Actions
{
    /// <summary>
    /// Representa a resposta estruturada retornada pela LLM.
    /// Cont�m p�ginas de di�logo e uma a��o opcional a ser executada.
    /// </summary>
    [System.Serializable]
    public class LLMResponseAction
    {
        /// <summary>
        /// Lista de p�ginas de di�logo (m�x. 150 caracteres cada).
        /// </summary>
        [JsonProperty("pages")]
        public List<string> Pages { get; set; }

        /// <summary>
        /// A��o opcional a ser executada (seguir, dar item, etc.).
        /// </summary>
        [JsonProperty("action")]
        public LLMAction Action { get; set; }

        public LLMResponseAction()
        {
            Pages = new List<string>();
            Action = new LLMAction();
        }
    }
}