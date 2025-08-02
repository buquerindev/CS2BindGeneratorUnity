using NUnit.Framework;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CFGManager : MonoBehaviour
{
    private string cfg_commands = "https://buquerindev.github.io/CS2ConfigGenerator/appdata/practice/practice.txt";

    [SerializeField] private JSONLoader JSONLoader;
    [SerializeField] private GameObject practiceCommandPanelPrefab;
    [SerializeField] private Transform commandPanelsTransform;

    private List<PracticeCommand> commands = new List<PracticeCommand>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JSONLoader.LoadFile(cfg_commands, (text) => {
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
}
