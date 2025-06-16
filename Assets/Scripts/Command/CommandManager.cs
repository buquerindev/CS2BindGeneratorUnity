using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class CommandManager : MonoBehaviour
{
    [SerializeField] private JSONLoader JSONLoader;

    private CommandList commandList;

    private readonly string jsonURL = "https://buquerindev.github.io/CS2BindGeneratorUnity/commands.json";
    //public string jsonContent;

    private void Start()
    {
        JSONLoader.LoadJSON(jsonURL, OnJSONReceived);
    }

    private void OnJSONReceived(string jsonText)
    {
        Debug.Log("Leyendo JSON");
        commandList = JsonUtility.FromJson<CommandList>(jsonText);
        foreach (Command cmd in commandList.commands)
        {
            Debug.Log(cmd.name);
        }
    }

}
