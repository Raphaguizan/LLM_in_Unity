using UnityEngine;


namespace Guizan.LLM.Embedding
{
    public static class NPCMemoryLoader
    {
        public static string LoadNPCStory(string filenameWithoutExtension)
        {
            TextAsset textAsset = Resources.Load<TextAsset>($"NPCsHistories/{filenameWithoutExtension}");
            if (textAsset == null)
            {
                Debug.LogError($"Erro: não foi possível carregar {filenameWithoutExtension} de Resources/NPCsHistories.");
                return null;
            }

            return textAsset.text;
        }
    }
}