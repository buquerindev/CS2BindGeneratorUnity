using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JSONLoader : MonoBehaviour
{

    public void LoadFile(string url, Action<String> onSuccess)
    {
        StartCoroutine(DownloadFile(url, onSuccess));
    }

    private IEnumerator DownloadFile(string fileURL, Action<string> onSuccess)
    {
        UnityWebRequest request = UnityWebRequest.Get(fileURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error en el request" + request.error);
        }
        else
        {
            string jsonText = request.downloadHandler.text;
            if (string.IsNullOrWhiteSpace(jsonText))
            {
                Debug.LogError("JSON vacío o inválido recibido desde " + fileURL);
                yield break;
            }

            try
            {
                onSuccess?.Invoke(jsonText);
            }
            catch (Exception ex)
            {
                Debug.LogError("Excepción al procesar JSON: " + ex.Message);
            }
        }

    }
}
