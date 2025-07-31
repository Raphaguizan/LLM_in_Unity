using UnityEngine;

namespace Guizan.LLM.Memory
{
    [RequireComponent(typeof(LLMAgent))]
    public class MemoryManager : MonoBehaviour
    {
        [SerializeField]
        private int memoryTotalLenght = 50;


        private LLMAgent agent;
        private AgentConfigs llmAgentConfigs;
        private Message lastAssistantMessage;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            agent = GetComponent<LLMAgent>();
            llmAgentConfigs = agent.AgentConfigs;
            llmAgentConfigs.MessagesChangeEvent.AddListener(VerifyMemoryLenght);
        }

        private void VerifyMemoryLenght(string role)
        {
            if (!role.Equals("assistant"))
                return;

            if (llmAgentConfigs.Lenght >= memoryTotalLenght)
                ResumeMemory();
        }

        private void ResumeMemory()
        {
            lastAssistantMessage = GetLastAssistantMessage();
            string resumeText = "Voc� est� interagindo com um jogador por meio de um personagem (NPC) em um jogo. O que foi conversado at� agora n�o ser� enviado novamente. A partir desta mensagem, quero que voc� gere um resumo conciso da conversa anterior.\r\n\r\nEsse resumo deve servir como uma mem�ria para o NPC, contendo apenas os fatos importantes, inten��es, decis�es ou sentimentos relevantes que o jogador demonstrou.\r\n\r\nIgnore cumprimentos, piadas ou partes irrelevantes.\r\n\r\nEscreva o resumo como se fosse uma nota pessoal do NPC para lembrar o que aconteceu at� agora. Use frases curtas, diretas, no estilo de t�picos.\r\n\r\nExemplo:\r\n- O jogador contou que foi expulso da col�nia Aurora por questionar a lideran�a.\r\n- Est� procurando pistas sobre um artefato chamado Prisma Sombrio.\r\n- Pediu ajuda para encontrar um engenheiro chamado Drax.\r\n\r\nAgora, por favor, resuma a conversa at� este ponto.\r\n";
            GroqLLM.SendMessageToLLM(llmAgentConfigs, new("user", resumeText));
            GroqLLM.ResponseEvent.AddListener(ReceiveResume);
        }

        private void ReceiveResume(ResponseLLM response)
        {
            if (response.AgentID != llmAgentConfigs.AgentID)
                return;

            llmAgentConfigs.ResetMessages();
            llmAgentConfigs.AddMessage(new("system", response.responseText));

            if (lastAssistantMessage != null)
                llmAgentConfigs.AddMessage(lastAssistantMessage);
            lastAssistantMessage = null;

            GroqLLM.ResponseEvent.RemoveListener(ReceiveResume);
        }

        private Message GetLastAssistantMessage()
        {
            for(int i = llmAgentConfigs.Messages.Count - 1; i >= 0; i--)
            {
                if (llmAgentConfigs.Messages[i].role.Equals("assistant"))
                    return llmAgentConfigs.Messages[i];
            }
            return null;
        }

        private void OnDestroy()
        {
            llmAgentConfigs.MessagesChangeEvent.RemoveAllListeners();
        }
    }
}