using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class CommandManager : MonoBehaviour
{
    [SerializeField] private JSONLoader JSONLoader;

    private CommandList commandList = new CommandList();

    private readonly string jsonURL = "https://buquerindev.github.io/CS2BindGeneratorUnity/commands.json";

    [SerializeField] private GameObject commandPanelPrefab;
    [SerializeField] private Transform commandPanelContainer;
    

    private void Start()
    {
        JSONLoader.LoadJSON(jsonURL, OnJSONReceived);
    }

    private void OnJSONReceived(string jsonText)
    {
        JObject root = JObject.Parse(jsonText);
        JArray commands = (JArray)root["commands"];

        foreach (JObject command in commands)
        {
            string type = command.Value<string>("type");
            Command cmd = new();

            cmd.name = command.Value<string>("name");
            cmd.ingameName = command.Value<string>("ingameName");
            cmd.description = command.Value<string>("description");
            cmd.category = command.Value<string>("category");
            cmd.subcategory = command.Value<string>("subcategory");
            cmd.type = type;

            if (type == "bool")
            {
                bool defaultValue;
                string boolString = command.Value<string>("defaultValue");
                if (boolString == "0") defaultValue = false; else defaultValue = true;
                cmd.defaultValue = defaultValue;
                cmd.selectedValue = cmd.defaultValue;
                commandList.commands.Add(cmd);
                continue;
            }

            if (type == "enum") 
            {
                cmd.enumValues = new List<int>();
                cmd.enumNames = new List<string>();
                cmd.defaultValue = command.Value<int>("defaultValue");

                // The values are like 0/1/2/3/4 -> Put them into a string and Split("/")
                string enumValuesString = command.Value<string>("enumValues");
                string[] enumValues = enumValuesString.Split("/");
                foreach (string enumValue in enumValues)
                {
                    cmd.enumValues.Add(int.Parse(enumValue));
                }

                // Same as before string1/string2/string3
                string enumNamesString = command.Value<string>("enumNames");
                string[] enumNames = enumNamesString.Split("/");
                foreach (string enumName in enumNames)
                {
                    cmd.enumNames.Add(enumName);
                }
                commandList.commands.Add(cmd);
                continue;
            }

            if (type == "string")
            {
                cmd.options = new List<string>();
                cmd.optionsNames = new List<string>();
                cmd.defaultValue = command.Value<string>("defaultValue");

                // The values are like +attack/+jump/drop... -> Put them into a string and Split("/")
                string optionsString = command.Value<string>("options");
                string[] options = optionsString.Split("/");
                foreach (string option in options)
                {
                    cmd.options.Add(option);
                }

                // Same as before string1/string2/string3
                string optionsNamesString = command.Value<string>("optionsNames");
                string[] optionsNames = optionsNamesString.Split("/");
                foreach (string option in optionsNames)
                {
                    cmd.optionsNames.Add(option);
                }
                commandList.commands.Add(cmd);
                continue;
            }
            if (type == "int")
            {
                cmd.min = command.Value<int>("min");
                cmd.max = command.Value<int>("max");
                cmd.defaultValue = command.Value<int>("defaultValue");
                commandList.commands.Add(cmd);
                continue;
            }
            if (type == "float")
            {
                cmd.min = command.Value<float>("min");
                cmd.max = command.Value<float>("max");
                cmd.defaultValue = command.Value<float>("defaultValue");
                commandList.commands.Add(cmd);
                continue;
            }
        }
        InitializeCommandPanels();
    }

    private void InitializeCommandPanels()
    {
        var orderedCommands = commandList.commands
        .OrderBy(cmd => cmd.category)
        .ThenBy(cmd => cmd.subcategory)
        .ToList();

        foreach (Command cmd in orderedCommands)
        {
            CommandPanel cmdPanel = Instantiate(commandPanelPrefab, commandPanelContainer).GetComponent<CommandPanel>();
            cmdPanel.SetCommand(cmd);
        }
    }
}
