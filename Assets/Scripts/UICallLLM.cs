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
    private GroqLLM LLMManager;


    private bool WaitResponse = false;

    private void OnEnable()
    {
        sendButton.onClick.AddListener(CallLLM);
        if (LLMManager == null)
        {
            Debug.LogError("manager não encontrado");
            return;
        }
        LLMManager.ResponseEvent.AddListener(ReceiveResponse);
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
        if (LLMManager == null)
        {
            Debug.LogError("manager não encontrado");
            return;
        }

        string text = inputField.text;
        if (text.Equals(string.Empty))
            return;

        // send data to script
        LLMManager.SendMessageToLLM(text);
        //Debug.Log("ENVIOU: "+text);
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

    private void ReceiveResponse(ResponseLLM response)
    {
        if (response.function == ResponseFunction.System)
            return;

        if(response.type == ResponseType.Success)
        {
            PrintLLMAnswer(response.responseText);
        }
        else
        {
            Debug.LogError(response.responseText);
        }
        ChangeWaitResponse(false);
    }

    private void PrintLLMAnswer(string resp)
    {
        textBox.text = resp;
    }
    private void OnDisable()
    {
        sendButton.onClick.RemoveListener(CallLLM);
        LLMManager.ResponseEvent.RemoveListener(ReceiveResponse);
    }
}
