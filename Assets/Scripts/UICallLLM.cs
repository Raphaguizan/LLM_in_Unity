using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Guizan.LLM;
using NaughtyAttributes;
using Guizan.LLM.Agent;

public class UICallLLM : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI textBox;
    [SerializeField]
    private Button sendButton;
    [SerializeField]
    private Button startButton;

    [Space]
    [SerializeField]
    private LLMAgent agent;
    [SerializeField]
    private AgentTalkManager agentTalk;


    private bool WaitResponse = false;

    private void OnEnable()
    {
        sendButton.onClick.AddListener(CallLLM);
        startButton.onClick.AddListener(ConversationToggle);
        if (agent == null)
        {
            Debug.LogError("Agente não encontrado");
            return;
        }
        agent.AnswerEvent.AddListener(PrintLLMAnswer);
    }

    private void ConversationToggle()
    {
        if (agentTalk.InConversation)
        {
            agentTalk.EndConversation();
            StartCoroutine(SetStartBtnArt(false));
        }
        else
        {
            agentTalk.StartConversation();
            StartCoroutine(SetStartBtnArt(true));
        }
    }

    private IEnumerator SetStartBtnArt(bool status)
    {
        yield return new WaitUntil(()=> agentTalk.InConversation == status);
        ColorBlock cb = startButton.colors;
        TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
        if (!status)
        {
            cb.normalColor = Color.gray9;
            cb.selectedColor = Color.gray9;
            buttonText.color = Color.black;
            buttonText.text = "Start";
        }
        else
        {
            cb.normalColor = Color.gray;
            cb.selectedColor = Color.gray;
            buttonText.color = Color.white;
            buttonText.text = "End";
        }
        startButton.colors = cb;
    }

    void Start()
    {
        textBox.text = "";
        inputField.text = "";
    }

    private void OnSubmit()
    {
        if (!WaitResponse)
        {
            CallLLM();
        }
    }

    private void CallLLM()
    {
        if (agent == null)
        {
            Debug.LogError("Agente não encontrado");
            return;
        }

        string text = inputField.text;
        if (text.Equals(string.Empty))
            return;

        // send data to script
        //agent.ReceiveText(text);
        agentTalk.SendMessage(text, callback: PrintLLMAnswer);
        ChangeWaitResponse(true);
        //MockReponseTime(3f);
    }

    private void MockReponseTime(float time)
    {
        StartCoroutine(MockResponseTimeCourotine(time));
    }

    private IEnumerator MockResponseTimeCourotine(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeWaitResponse(false);
    }

    private void ChangeWaitResponse(bool val)
    {
        WaitResponse = val;
        sendButton.interactable = !val;
        if (!val) inputField.text = "";
    }

    private void PrintLLMAnswer(string resp)
    {
        textBox.text = resp;
        ChangeWaitResponse(false);
    }
    private void OnDisable()
    {
        sendButton.onClick.RemoveListener(CallLLM);
        startButton.onClick.RemoveListener(ConversationToggle);
        agent.AnswerEvent.RemoveListener(PrintLLMAnswer);
    }
}
