using UnityEngine;

namespace Guizan.LLM.Memory
{
    public class MemoryManager : MonoBehaviour
    {
        [SerializeField]
        private GroqLLM LLMManager;

        [SerializeField]
        private int memoryTotalLenght = 50;


        private ChatRequest llmRequest;
        private Message lastAssistantMessage;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            LLMManager.ResponseEvent.AddListener(VerifyMemoryLenght);
            LLMManager.ResponseEvent.AddListener(ReceiveResume);

            llmRequest = LLMManager.RequestCofig;
        }

        private void VerifyMemoryLenght(ResponseLLM response)
        {
            if(llmRequest.Lenght >= memoryTotalLenght 
                && response.function == ResponseFunction.User)
                ResumeMemory();
        }

        private void ResumeMemory()
        {
            lastAssistantMessage = GetLastAssistantMessage();
            string resumeText = "Você está interagindo com um jogador por meio de um personagem (NPC) em um jogo. O que foi conversado até agora não será enviado novamente. A partir desta mensagem, quero que você gere um resumo conciso da conversa anterior.\r\n\r\nEsse resumo deve servir como uma memória para o NPC, contendo apenas os fatos importantes, intenções, decisões ou sentimentos relevantes que o jogador demonstrou.\r\n\r\nIgnore cumprimentos, piadas ou partes irrelevantes.\r\n\r\nEscreva o resumo como se fosse uma nota pessoal do NPC para lembrar o que aconteceu até agora. Use frases curtas, diretas, no estilo de tópicos.\r\n\r\nExemplo:\r\n- O jogador contou que foi expulso da colônia Aurora por questionar a liderança.\r\n- Está procurando pistas sobre um artefato chamado Prisma Sombrio.\r\n- Pediu ajuda para encontrar um engenheiro chamado Drax.\r\n\r\nAgora, por favor, resuma a conversa até este ponto.\r\n";
            LLMManager.SendMessageToLLM(resumeText, ResponseFunction.System);
        }

        private void ReceiveResume(ResponseLLM response)
        {
            if (response.function == ResponseFunction.User)
                return;

            llmRequest.ResetMessages();
            llmRequest.AddMessage(new("system", response.responseText));

            if (lastAssistantMessage != null)
                llmRequest.AddMessage(lastAssistantMessage);
            lastAssistantMessage = null;
        }

        private Message GetLastAssistantMessage()
        {
            for(int i = llmRequest.Messages.Count - 1; i >= 0; i--)
            {
                if (llmRequest.Messages[i].role.Equals("assistant"))
                    return llmRequest.Messages[i];
            }
            return null;
        }

        private void OnDestroy()
        {
            LLMManager.ResponseEvent.RemoveAllListeners();
        }
    }
}