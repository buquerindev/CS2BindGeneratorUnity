using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance;

    [SerializeField] private RemoteFileManager fileManager;

    private CommandList commandList = new CommandList();
    private List<CommandPanel> commandPanels = new List<CommandPanel>();

    private readonly string jsonURL = "commands.json";
    //private readonly string snd_formula_txt = "https://buquerindev.github.io/CS2ConfigGenerator/appdata/snd_formula.txt";
    //private readonly string snd_to_decimal_txt = "https://buquerindev.github.io/CS2ConfigGenerator/appdata/snd_to_decimal.txt";

    public List<string> snd_formula_commands;
    public List<string> snd_to_decimal_commands;

    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GameObject commandSeparatorPrefab;
    [SerializeField] private GameObject commandPanelPrefab;

    [SerializeField] private Transform audioPanelContainer;
    [SerializeField] private Transform gamePanelContainer;
    [SerializeField] private Transform keyboardMousePanelContainer;
    private Transform currentContainer;

    [SerializeField] private Button exportSettingsButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button gameButton;
    [SerializeField] private Button keyboardMouseButton;
    [SerializeField] private Button aliasButton;

    private List<Button> menusButtons;

    private Color buttonDefaultColor;
    private Color buttonSelectedColor;

    private ColorBlock selectedCB;
    private ColorBlock cb;

    private Dictionary<string, Transform> categoryContainers;

    private bool sndFormulaLoaded = false;
    private bool sndDecimalLoaded = false;

    // Alias

    [SerializeField] private Button addAliasButton;
    [SerializeField] private GameObject aliasPanelPrefab;
    [SerializeField] private Transform aliasPanelContainer;
    public List<Alias> aliases = new List<Alias>();

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
        ColorUtility.TryParseHtmlString("#00000096", out buttonDefaultColor);
        ColorUtility.TryParseHtmlString("#000000C8", out buttonSelectedColor);

        menusButtons = new List<Button> {
            audioButton,
            gameButton,
            keyboardMouseButton,
            aliasButton
        };

        currentContainer = audioPanelContainer;
        scrollRect.content = currentContainer as RectTransform;
        InitializeContainerDictionary();

        fileManager.LoadFile("snd_formula.txt", (text) => {
            SNDFormulaList(text);
            sndFormulaLoaded = true;
            TryLoadJSON();
        });
        fileManager.LoadFile("snd_to_decimal.txt", (text) => {
            SNDDecimalList(text);
            sndDecimalLoaded = true;
            TryLoadJSON();
        });


        // Buttons
        addAliasButton.onClick.AddListener(AddAliasPanel);
        exportSettingsButton.onClick.AddListener(ExportSettings);
        audioButton.onClick.AddListener(() => SwitchContainer(audioPanelContainer, audioButton));
        gameButton.onClick.AddListener(() => SwitchContainer(gamePanelContainer, gameButton));
        keyboardMouseButton.onClick.AddListener(() => SwitchContainer(keyboardMousePanelContainer, keyboardMouseButton));
        aliasButton.onClick.AddListener(() => SwitchContainer(aliasPanelContainer, aliasButton));
        LoadAliases();
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

    private void LoadAliases()
    {
        Alias kill_entities = new Alias()
        {
            originalCommand = "ent_fire smokegrenade_projectile kill;ent_fire molotov_projectile kill;ent_fire inferno kill;ent_fire flashbang_projectile kill;ent_fire hegrenade_projectile kill;ent_fire decoy_projectile kill;stopsound;say Killed all utility entities!",
            aliasCommand = "kill_entities"
        };

        aliases.Add(kill_entities);

        string filePath = Path.Combine(Application.persistentDataPath, "settings.txt");
        if (!File.Exists(filePath))
            return;

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (line.StartsWith("alias|"))
            {
                string[] parts = line.Split('|');
                if (parts[2] == "kill_entities") continue;
                AliasPanel aliasPanel = Instantiate(aliasPanelPrefab, aliasPanelContainer).GetComponent<AliasPanel>();
                aliasPanel.transform.SetSiblingIndex(aliasPanelContainer.childCount - 2);
                aliasPanel.Initialize(parts[1], parts[2]);
            }
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

            // Also save aliases
            foreach (Alias alias in aliases)
            {
                if (alias.IsWritten())
                {
                    writer.WriteLine($"alias|{alias.originalCommand}|{alias.aliasCommand}");
                }
            }
        }
    }

    private string SelectFolder()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);
        if (paths.Length > 0)
        {
            string folderPath = paths[0];
            Debug.Log("Selected folder: " + folderPath);
            return folderPath;
        }
        return null;
    }

    private void ExportSettings()
    {
        SaveSettings();

        string filePath = SelectFolder();

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning("No folder selected for export.");
            return;
        }

        filePath = Path.Combine(filePath, "settings.cfg");

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
            writer.WriteLine();
            writer.WriteLine($"//========== ALIASES ==========");
            foreach (Alias alias in aliases)
            {
                if (alias.IsWritten())
                {
                    string line = $"alias \"{alias.originalCommand}\" \"{alias.aliasCommand}\"";
                    writer.WriteLine(line);
                }
            }
        }
    }

    private void SwitchContainer(Transform newContainer, Button button)
    {
        foreach (Button btn in menusButtons)
        {
            cb = btn.colors;

            cb.normalColor = buttonDefaultColor;
            cb.highlightedColor = buttonDefaultColor;
            cb.pressedColor = buttonDefaultColor;
            cb.selectedColor = buttonDefaultColor;
            cb.disabledColor = buttonDefaultColor;

            btn.colors = cb;
        }

        // Ahora aplicar los colores "activos" al bot�n seleccionado
        selectedCB = button.colors;

        selectedCB.normalColor = buttonSelectedColor;
        selectedCB.highlightedColor = buttonSelectedColor;
        selectedCB.pressedColor = buttonSelectedColor;
        selectedCB.selectedColor = buttonSelectedColor;
        selectedCB.disabledColor = buttonSelectedColor;

        button.colors = selectedCB;

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
                { "kbmouse", keyboardMousePanelContainer }
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
            fileManager.LoadFile(jsonURL, (text) =>
            {
                Debug.Log("JSON de commands recibido, comenzando a instanciar paneles");
                OnJSONReceived(text);
            });
        }
    }

    #region Alias

    private void AddAliasPanel()
    {
        AliasPanel aliasPanel = Instantiate(aliasPanelPrefab, aliasPanelContainer).GetComponent<AliasPanel>();
        int insertIndex = aliasPanelContainer.childCount - 2;
        aliasPanel.transform.SetSiblingIndex(insertIndex);
        aliasPanel.Initialize();
    }

    #endregion
}
