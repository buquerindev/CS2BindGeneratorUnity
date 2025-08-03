using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RemoteFileManager : MonoBehaviour
{
    [System.Serializable]
    public class RemoteFile
    {
        public string fileName;      // "practice_commands.txt"
        public string url;           // URL al archivo
    }

    public List<RemoteFile> filesToSync;
    public string localPath = "Assets/Resources/RemoteFiles/";
    public int maxRetries = 5;

    public void LoadFile(string fileName, Action<string> onSuccess)
    {
        StartCoroutine(EnsureFileReady(fileName, onSuccess));
    }

    private IEnumerator EnsureFileReady(string fileName, Action<string> onSuccess)
    {
        RemoteFile file = filesToSync.Find(f => f.fileName == fileName);
        if (file == null)
        {
            Debug.LogError("Archivo no encontrado en configuración: " + fileName);
            yield break;
        }

        string localFullPath = Path.Combine(localPath, file.fileName);
        bool success = false;
        string content = null;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            UnityWebRequest request = UnityWebRequest.Get(file.url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                content = request.downloadHandler.text;

                if (File.Exists(localFullPath))
                {
                    string localContent = File.ReadAllText(localFullPath);
                    if (localContent != content)
                    {
                        File.WriteAllText(localFullPath, content);
                        Debug.Log($"Archivo actualizado: {file.fileName}");
                    }
                }
                else
                {
                    Directory.CreateDirectory(localPath);
                    File.WriteAllText(localFullPath, content);
                    Debug.Log($"Archivo guardado localmente: {file.fileName}");
                }

                success = true;
                break;
            }
            else
            {
                Debug.LogWarning($"Intento {attempt} fallido al descargar {file.fileName}: {request.error}");
                yield return new WaitForSeconds(1f); // pequeño delay antes del siguiente intento
            }
        }

        // Fallback
        if (!success)
        {
            if (File.Exists(localFullPath))
            {
                content = File.ReadAllText(localFullPath);
                Debug.LogWarning($"Usando archivo local (no se pudo obtener el remoto): {file.fileName}");
            }
            else
            {
                Debug.LogError($"No se pudo obtener ni archivo remoto ni local para: {file.fileName}");
                yield break;
            }
        }

        onSuccess?.Invoke(content);
    }
}
