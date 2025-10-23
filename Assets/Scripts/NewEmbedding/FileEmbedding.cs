using Guizan.LLM.Utils;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;

namespace Guizan.LLM.Embedding
{
    [CreateAssetMenu(fileName = "newEmbeddingFile", menuName ="Embedding/File")]
    public class FileEmbedding : ScriptableObject
    {
        [SerializeField]
        private TextAsset archive;

        private TextAsset testChange;

        private string FileName => archive.name+"-Embedding";

        private AgentEmbedding myEmbedding = default;

        public void GetEmbeddings(Action<AgentEmbedding> onResponse)
        {
            if (archive == null)
            {
                Debug.LogWarning("arquivo nulo");
                onResponse?.Invoke(default);
                return;
            }

            // retorna o embedding caso esteja na mem�ria
            if(!myEmbedding.IsEmpty())
            {
                Debug.LogWarning(myEmbedding);
                Debug.LogWarning("embedding est� na mem�ria: "+myEmbedding.ToString());
                onResponse?.Invoke(myEmbedding);
                return;
            }

            // retorna o embedding caso esteja salvo e carrega ele para mem�ria
            myEmbedding = AgentJSONSaver<AgentEmbedding>.LoadJSON(FileName, SavePathFolder.embedding);
            if (myEmbedding != default) 
            {
                Debug.Log("Carregando embedding da Hist�ria salva.");
                onResponse?.Invoke(myEmbedding);
                return;
            }

            // retorna um novo embedding gerado, salva e carrega para a mem�ria
            Debug.Log("Gerando novo embedding da Hist�ria.");
            List<string> storyChunks = MakeChunks(archive.text);
            CohereEmbeddingClient.RequestEmbeddings(storyChunks, (embedding, type) => ReceiveEmbeddinResponse(embedding, type, onResponse));
        }

        private void ReceiveEmbeddinResponse(AgentEmbedding embedding, ResponseType type, Action<AgentEmbedding> onResponse)
        {
            if (type == ResponseType.Error)
            {
                Debug.LogError("Error! ao receber os novos embeddins");
                return;
            }

            myEmbedding = embedding;
            SaveEmbedding(embedding);
            onResponse?.Invoke(embedding);
        }

        private void SaveEmbedding(AgentEmbedding embeddins)
        {
            AgentJSONSaver<AgentEmbedding>.SaveJSON(FileName, embeddins, SavePathFolder.embedding);
        }

        public static List<string> MakeChunks(string text)
        {
            return text.SplitTextIntoChunksWithOverlap(256, 32);
        }

        private void ResetSavedEmbeddins()
        {
            AgentJSONSaver<AgentEmbedding>.ClearJSON(FileName);
            myEmbedding.SetEmpty();
        }

        private void OnValidate()
        {
            if (archive == testChange)
                return;

            testChange = archive;
            ResetSavedEmbeddins();
        }

        private void Reset()
        {
            ResetSavedEmbeddins();
            testChange = null; 
            archive = null;
        }
    }
}