using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JSONLoader : MonoBehaviour
{

    public void LoadJSON(string url, Action<String> onSuccess)
    {
        StartCoroutine(DownloadJSON(url, onSuccess));
    }

    private IEnumerator DownloadJSON(string jsonURL, Action<string> onSuccess)
    {
        UnityWebRequest request = UnityWebRequest.Get(jsonURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error en el request" + request.error);
        }
        else
        {
            string jsonText = request.downloadHandler.text;
            onSuccess.Invoke(jsonText);
        }
    }
}
