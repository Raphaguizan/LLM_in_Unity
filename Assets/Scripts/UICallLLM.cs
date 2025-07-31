using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Guizan.LLM;
using NaughtyAttributes;

public class UICallLLM : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI textBox;
    [SerializeField]
    private Button sendButton;

    [Space]
    [SerializeField]
    private LLMAgent agent;


    private bool WaitResponse = false;

    private void OnEnable()
    {
        sendButton.onClick.AddListener(CallLLM);
        if (agent == null)
        {
            Debug.LogError("Agente não encontrado");
            return;
        }
        agent.AnswerEvent.AddListener(PrintLLMAnswer);
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
        agent.ReceiveText(text);
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
        agent.AnswerEvent.RemoveListener(PrintLLMAnswer);
    }
}
