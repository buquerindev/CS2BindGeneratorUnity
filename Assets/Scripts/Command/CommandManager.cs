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
using UnityEngine.UI;
using System.Globalization;

public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance;

    [SerializeField] private JSONLoader JSONLoader;

    private CommandList commandList = new CommandList();
    private List<CommandPanel> commandPanels = new List<CommandPanel>();

    private readonly string jsonURL = "https://buquerindev.github.io/CS2BindGeneratorUnity/commands.json";
    private readonly string snd_formula_txt = "https://buquerindev.github.io/CS2BindGeneratorUnity/snd_formula.txt";
    private readonly string snd_to_decimal_txt = "https://buquerindev.github.io/CS2BindGeneratorUnity/snd_to_decimal.txt";

    public List<string> snd_formula_commands;
    public List<string> snd_to_decimal_commands;

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

    private bool sndFormulaLoaded = false;
    private bool sndDecimalLoaded = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        currentContainer = audioPanelContainer;
        scrollRect.content = currentContainer as RectTransform;
        InitializeContainerDictionary();

        JSONLoader.LoadFile(snd_formula_txt, (text) => {
            SNDFormulaList(text);
            sndFormulaLoaded = true;
            TryLoadJSON();
        });
        JSONLoader.LoadFile(snd_to_decimal_txt, (text) => {
            SNDDecimalList(text);
            sndDecimalLoaded = true;
            TryLoadJSON();
        });


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
                    commandPanels.Add(cmdPanel);
                }
            }
        }

        string filePath = Path.Combine(Application.persistentDataPath, "settings.txt");
        if (!File.Exists(filePath))
            return;

        string[] lines = File.ReadAllLines(filePath);

        foreach (CommandPanel commandPanel in commandPanels)
        {
            commandPanel.LoadCommand(lines);
        }
    }

    private void SaveSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "settings.txt");

        Debug.Log("Created file: " + filePath);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Command command in commandList.commands)
            {
                writer.WriteLine($"{command.name}|{command.selectedValue}");
            }
        }
    }

    private void ExportSettings()
    {
        string filePath = null;
        string folderPath = EditorUtility.OpenFolderPanel("Select Export Folder", "", "");

        if (!string.IsNullOrEmpty(folderPath))
        {
            filePath = System.IO.Path.Combine(folderPath, "config.cfg");

            System.IO.File.WriteAllText(filePath, "// Tu contenido aqu�...");
            Debug.Log("Archivo exportado en: " + filePath);
        }
        else
        {
            Debug.Log("Exportaci�n cancelada.");
            return;
        }

        SaveSettings();

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
                        string left = (cmd.convertedValue == null ? 
                            $"{cmd.name} {cmd.selectedValue.ToString()}" 
                            : $"{cmd.name} {cmd.convertedValue.ToString()}");
                        left = left.PadRight(maxLeftWidth + 4);
                        left = left.Replace(',', '.');
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

    private void SNDFormulaList(string file)
    {
        snd_formula_commands = new List<string>();

        // Divide el contenido en l�neas
        string[] lines = file.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        // Comprueba que hay al menos dos l�neas
        if (lines.Length > 1)
        {
            string[] values = lines[1].Split(',');
            snd_formula_commands.AddRange(values);
        }
        foreach (string line in snd_formula_commands)
        {
            Debug.Log(line);
        }
    }

    private void SNDDecimalList(string file)
    {
        snd_to_decimal_commands = new List<string>();

        // Divide el contenido en l�neas
        string[] lines = file.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        // Comprueba que hay al menos dos l�neas
        if (lines.Length > 1)
        {
            string[] values = lines[1].Split(',');
            snd_to_decimal_commands.AddRange(values);
        }
    }

    private void TryLoadJSON()
    {
        if (sndFormulaLoaded && sndDecimalLoaded)
        {
            JSONLoader.LoadFile(jsonURL, OnJSONReceived);
        }
    }
}
