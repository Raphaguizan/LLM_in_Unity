using UnityEngine;

[CreateAssetMenu(fileName = "APIKeyConfig", menuName = "Config/API Key")]

public class APIKeyConfig : ScriptableObject
{
    [SerializeField]
    private string apiKey;

    public string Key => apiKey;
}
