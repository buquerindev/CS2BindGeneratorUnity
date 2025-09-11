using NUnit.Framework;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CFGManager : MonoBehaviour
{
    private string cfg_commands = "practice.txt";

    [SerializeField] private RemoteFileManager fileManager;
    [SerializeField] private GameObject practiceCommandPanelPrefab;
    [SerializeField] private Transform commandPanelsTransform;

    [SerializeField] private Transform practicePanelContainer;
    [SerializeField] private Transform instaSmokesContainer;

    private List<PracticeCommand> commands = new List<PracticeCommand>();

    [SerializeField] private Button practiceButton;
    [SerializeField] private Button instaSmokesButton;
    
    private List<Button> menusButtons;
    private Transform currentContainer;

    private Color buttonDefaultColor;
    private Color buttonSelectedColor;

    private ColorBlock selectedCB;
    private ColorBlock cb;

    [SerializeField] private ScrollRect scrollRect;

    private Dictionary<string, Transform> categoryContainers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorUtility.TryParseHtmlString("#00000064", out buttonDefaultColor);
        ColorUtility.TryParseHtmlString("#000000C8", out buttonSelectedColor);

        menusButtons = new List<Button> {
            practiceButton,
            instaSmokesButton
        };

        currentContainer = practicePanelContainer;
        scrollRect.content = currentContainer as RectTransform;
        InitializeContainerDictionary();

        fileManager.LoadFile(cfg_commands, (text) => {
            OnTXTReceived(text);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTXTReceived(string text)
    {
        Debug.Log("Archivo TXT cargado correctamente.");
        foreach (string line in text.Split('\n'))
        {
            string trimmedLine = line.Trim();
            string[] parts = trimmedLine.Split('|');
            PracticeCommand command = new PracticeCommand();
            Debug.Log("Procesando línea: " + parts[0]);
            command.commandName = parts[0];
            command.ingameName = parts[1];
            command.type = parts[3];
            command.description = parts[4];
            if (command.type == "bool")
            {
                if (parts[2] == "1") command.defaultValue = true;
                else if (parts[2] == "0") command.defaultValue = false;
                else command.defaultValue = bool.Parse(parts[2]);

                command.selectedValue = command.defaultValue;
            }
            else 
            {
                command.defaultValue = parts[2];
                command.selectedValue = command.defaultValue;
            }
            commands.Add(command);
        }
        Debug.Log("Total de comandos cargados: " + commands.Count);
        InstantiatePanels();
    }

    private void InstantiatePanels()
    {
        foreach (PracticeCommand command in commands)
        {
            Debug.Log("Instanciando panel para comando: " + command.commandName);
            PracticeCommandPanel panel = Instantiate(practiceCommandPanelPrefab, commandPanelsTransform).GetComponent<PracticeCommandPanel>();
            panel.Initialize(command);
        }
    }


    private void ExportCFG()
    {

        string filePath = SelectFolder();

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning("No folder selected for export.");
            return;
        }

        filePath = Path.Combine(filePath, "practice.cfg");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("sv_cheats 1");
        
            // Al final
            writer.WriteLine("bot_kick");
            writer.WriteLine("mp_warmup_end");
            writer.WriteLine("mp_restartgame 1"); 

            foreach (PracticeCommand command in commands)
            {
                writer.WriteLine($"{command.commandName} {command.selectedValue}");
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
                { "practice", practicePanelContainer },
                //{ "instaSmokes", instaSmokesContainer }
            };

    }
}
