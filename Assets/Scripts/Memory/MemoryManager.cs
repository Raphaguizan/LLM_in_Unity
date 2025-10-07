using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Memory
{
    [RequireComponent(typeof(LLMAgent))]
    public class MemoryManager : MonoBehaviour
    {
        [SerializeField]
        private int memoryTotalLenght = 50;

        [SerializeField, ResizableTextArea]
        private string sumaryPrompt = "Voc� est� interagindo com um jogador por meio de um personagem (NPC) em um jogo. O que foi conversado at� agora n�o ser� enviado novamente. A partir desta mensagem, quero que voc� gere um resumo conciso da conversa anterior.\r\n\r\nEsse resumo deve servir como uma mem�ria para o NPC, contendo apenas os fatos importantes, inten��es, decis�es ou sentimentos relevantes que o jogador demonstrou.\r\n\r\nIgnore cumprimentos, piadas ou partes irrelevantes.\r\n\r\nEscreva o resumo como se fosse uma nota pessoal do NPC para lembrar o que aconteceu at� agora. Use frases curtas, diretas, no estilo de t�picos.\r\n\r\nExemplo:\r\n- O jogador contou que foi expulso da col�nia Aurora por questionar a lideran�a.\r\n- Est� procurando pistas sobre um artefato chamado Prisma Sombrio.\r\n- Pediu ajuda para encontrar um engenheiro chamado Drax.\r\n\r\nAgora, por favor, resuma a conversa at� este ponto.\r\n";

        private LLMAgent agent;
        private AgentConfigs llmAgentConfigs;
        private Message lastAssistantMessage;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            agent = GetComponent<LLMAgent>();
            llmAgentConfigs = agent.AgentConfigs;
            LoadMemory();
            llmAgentConfigs.MessagesChangeEvent.AddListener(VerifyMemoryLenght);
        }

        private void VerifyMemoryLenght(string role)
        {
            if (!role.Equals("assistant"))
                return;

            if (llmAgentConfigs.Lenght >= memoryTotalLenght)
                SumarizeMemory();
        }

        /// <summary>
        /// Faz um resumo da conversa sem esperar o limite m�ximo de mensagens.
        /// Quando o jogador parar de interagir com o NPC ele saber que isso aconteceu e n�o apenas
        /// continuar a conversa como se nada tivesse acontecido.
        /// </summary>
        /// <param name="resumeMessage">Mensagem do systema antes de fazer o resumo. exemplo: "O jogador foi embora e acabou a conversa."</param>
        public void MakeMemorySumary(string resumeMessage)
        {
            GroqLLM.SendMessageToLLM(llmAgentConfigs, new(MessageRole.system, resumeMessage));
            SumarizeMemory(false);
        }

        private void SumarizeMemory(bool keepLastMessage = true)
        {
            if(keepLastMessage)
                lastAssistantMessage = GetLastAssistantMessage();

            GroqLLM.SendMessageToLLM(llmAgentConfigs, new(MessageRole.user, sumaryPrompt));
            GroqLLM.ResponseEvent.AddListener(ReceiveSumary);
        }

        private void ReceiveSumary(ResponseLLM response)
        {
            if (response.AgentID != llmAgentConfigs.AgentID)
                return;

            llmAgentConfigs.ResetMessages();
            llmAgentConfigs.AddMessage(new(MessageRole.system, response.responseText));

            if (lastAssistantMessage != null)
                llmAgentConfigs.AddMessage(lastAssistantMessage);
            lastAssistantMessage = null;

            GroqLLM.ResponseEvent.RemoveListener(ReceiveSumary);
        }

        private Message GetLastAssistantMessage()
        {
            for(int i = llmAgentConfigs.Messages.Count - 1; i >= 0; i--)
            {
                if (llmAgentConfigs.Messages[i].role.Equals(MessageRole.assistant))
                    return llmAgentConfigs.Messages[i];
            }
            return null;
        }

        private void LoadMemory()
        {
            var loadData = AgentJSONSaver<List<Message>>.LoadJSON(llmAgentConfigs.AgentID, SavePathFolder.npc_Memory);
            if(loadData != default)
            {
                llmAgentConfigs.LoadMessages(loadData);
            }
        } 

        private void SaveMemory()
        {
            AgentJSONSaver<List<Message>>.SaveJSON(llmAgentConfigs.AgentID, llmAgentConfigs.Messages, SavePathFolder.npc_Memory);
        }

        [Button]
        public void ResetMemory()
        {
            AgentJSONSaver<List<Message>>.ClearJSON(llmAgentConfigs.AgentID, SavePathFolder.npc_Memory);
            llmAgentConfigs.ResetMessages();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
                SaveMemory();
        }
        private void OnDestroy()
        {
            llmAgentConfigs.MessagesChangeEvent.RemoveAllListeners();
            SaveMemory();
        }
    }
}