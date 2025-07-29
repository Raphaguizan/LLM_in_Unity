using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UICallLLM : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI textBox;
    [SerializeField]
    private Button sendButton;


    private bool WaitResponse = false;

    private void OnEnable()
    {
        sendButton.onClick.AddListener(CallLLM);
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
        string text = inputField.text;
        if (text.Equals(string.Empty))
            return;

        // send data to script
        Debug.Log("ENVIOU: "+text);
        ChangeWaitResponse(true);
        MockReponseTime(3f);
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
    }
    private void OnDisable()
    {
        sendButton.onClick.RemoveListener(CallLLM);
    }
}
