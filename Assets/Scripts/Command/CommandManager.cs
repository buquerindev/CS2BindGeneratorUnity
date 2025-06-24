using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour
{
    [SerializeField] private JSONLoader JSONLoader;

    private CommandList commandList = new CommandList();

    private readonly string jsonURL = "https://buquerindev.github.io/CS2BindGeneratorUnity/commands.json";

    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GameObject commandSeparatorPrefab;
    [SerializeField] private GameObject commandPanelPrefab;

    [SerializeField] private Transform audioPanelContainer;
    [SerializeField] private Transform gamePanelContainer;
    private Transform currentContainer;

    [SerializeField] private Button exportSettingsButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button gameButton;

    private Dictionary<string, Transform> categoryContainers;


    private void Start()
    {
        currentContainer = audioPanelContainer;
        scrollRect.content = currentContainer as RectTransform;
        InitializeContainerDictionary();

        JSONLoader.LoadJSON(jsonURL, OnJSONReceived);

        // Buttons
        exportSettingsButton.onClick.AddListener(ExportSettings);
        audioButton.onClick.AddListener(() => SwitchContainer(audioPanelContainer));
        gameButton.onClick.AddListener(() => SwitchContainer(gamePanelContainer));
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
                commandList.Add(cmd);
                continue;
            }

            if (type == "enum") 
            {
                cmd.enumValues = new List<int>();
                cmd.enumNames = new List<string>();
                cmd.defaultValue = command.Value<int>("defaultValue");
                cmd.selectedValue = cmd.defaultValue;

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
                commandList.Add(cmd);
                continue;
            }

            if (type == "string")
            {
                cmd.options = new List<string>();
                cmd.optionsNames = new List<string>();
                cmd.defaultValue = command.Value<string>("defaultValue");
                cmd.selectedValue = cmd.defaultValue;

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
                commandList.Add(cmd);
                continue;
            }

            if (type == "int")
            {
                cmd.min = command.Value<int>("min");
                cmd.max = command.Value<int>("max");
                cmd.defaultValue = command.Value<int>("defaultValue");
                cmd.selectedValue = cmd.defaultValue;
                commandList.Add(cmd);
                continue;
            }

            if (type == "float")
            {
                cmd.min = command.Value<float>("min");
                cmd.max = command.Value<float>("max");
                cmd.defaultValue = command.Value<float>("defaultValue");
                cmd.selectedValue = cmd.defaultValue;
                commandList.Add(cmd);
                continue;
            }
        }
        InitializeCommandPanels();
    }

    private void InitializeCommandPanels()
    {
        var groupedCommands = commandList.commands
            .GroupBy(cmd => cmd.category)
            .ToDictionary(
                group => group.Key,
                group => group
                    .GroupBy(cmd => cmd.subcategory)
                    .ToDictionary(subgroup => subgroup.Key, subgroup => subgroup.ToList())
            );

        foreach (var category in groupedCommands)
        {
            Transform targetTransform = categoryContainers.ContainsKey(category.Key.ToLower())
                ? categoryContainers[category.Key.ToLower()]
                : null;
            var subcategories = category.Value;
            foreach (var subcategory in subcategories)
            {
                var commands = subcategory.Value;
                Separator commandSeparator = Instantiate(commandSeparatorPrefab, targetTransform).GetComponent<Separator>();
                commandSeparator.SetName($"{category.Key.ToUpper()} - {subcategory.Key}");
                foreach(var cmd in commands)
                {
                    CommandPanel cmdPanel = Instantiate(commandPanelPrefab, targetTransform).GetComponent<CommandPanel>();
                    cmdPanel.SetCommand(cmd);
                }
            }
        }
    }

    private void ExportSettings()
    {
        // Create a .cfg file
        string filePath = Path.Combine(Application.dataPath, "ExportFiles/settings.cfg");

        // Get the biggest command to align the comments
        int maxLeftWidth = commandList.commands
            .Select(cmd => $"{cmd.name} {cmd.defaultValue}".Length)
            .Max();

        var groupedCommands = commandList.commands
            .GroupBy(cmd => cmd.category)
            .ToDictionary(
                group => group.Key,
                group => group
                    .GroupBy(cmd => cmd.subcategory)
                    .ToDictionary(subgroup => subgroup.Key, subgroup => subgroup.ToList())
            );


        //CommandList audioCommands = (CommandList)commandList.commands.GroupBy(cmd => cmd.category);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var categoryPair in groupedCommands)
            {
                var subcategories = categoryPair.Value;
                foreach (var subcategory in subcategories)
                {
                    writer.WriteLine();
                    writer.WriteLine($"//========== {categoryPair.Key.ToUpper()} - {subcategory.Key} ==========");

                    var commands = subcategory.Value;
                    foreach (var cmd in commands)
                    {
                        string left = $"{cmd.name} {cmd.selectedValue.ToString()}".PadRight(maxLeftWidth + 4);
                        string line = left + $"// {cmd.ingameName}";
                        writer.WriteLine(line);
                    }
                }
            }
        }
    }

    private void SwitchContainer(Transform newContainer)
    {
        currentContainer.gameObject.SetActive(false);
        newContainer.gameObject.SetActive(true);

        scrollRect.content = newContainer as RectTransform;
        currentContainer = newContainer;
    }

    private void InitializeContainerDictionary()
    {
        categoryContainers = new Dictionary<string, Transform>
            {
                { "game", gamePanelContainer },
                { "audio", audioPanelContainer },
            };

    }
}
