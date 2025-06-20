using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BindManager : MonoBehaviour
{
    [SerializeField] private JSONLoader JSONLoader;
    private BindList bindList = new();
    private readonly string jsonURL = "https://buquerindev.github.io/CS2BindGeneratorUnity/binds.json";

    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GameObject bindSeparatorPrefab;
    [SerializeField] private GameObject bindPanelPrefab;

    [SerializeField] private Transform controlsPanelContainer;
    [SerializeField] private Transform hiddenPanelContainer;
    private Transform currentContainer;

    [SerializeField] private Button controlsButton;
    [SerializeField] private Button hiddenButton;
    [SerializeField] private Button exportBindsButton;

    private Dictionary<string, Transform> categoryContainers;

    private void Start()
    {
        currentContainer = controlsPanelContainer;
        scrollRect.content = currentContainer as RectTransform;
        InitializeContainerDictionary();

        JSONLoader.LoadJSON(jsonURL, OnJSONReceived);

        controlsButton.onClick.AddListener(() => SwitchContainer(controlsPanelContainer));
        hiddenButton.onClick.AddListener(() => SwitchContainer(hiddenPanelContainer));
        exportBindsButton.onClick.AddListener(ExportBinds);
    }

    private void OnJSONReceived(string jsonText)
    {
        JObject root = JObject.Parse(jsonText);
        JArray binds = (JArray)root["binds"];

        foreach (JObject JObject in binds)
        {
            Bind bind = new();
            bind.name = JObject.Value<string>("name");
            bind.ingameName = JObject.Value<string>("ingameName");
            bind.description = JObject.Value<string>("description");
            bind.category = JObject.Value<string>("category");
            bind.subcategory = JObject.Value<string>("subcategory");

            bindList.Add(bind);
        }
        InitializeBindPanels();
    }

    private void InitializeBindPanels()
    {
        var groupedBinds = bindList.binds
            .GroupBy(bind => bind.category)
            .ToDictionary(
                group => group.Key,
                group => group
                    .GroupBy(bind => bind.subcategory)
                    .ToDictionary(subgroup => subgroup.Key, subgroup => subgroup.ToList())
            );

        foreach (var category in groupedBinds)
        {
            Transform targetTransform = categoryContainers.ContainsKey(category.Key.ToLower())
                ? categoryContainers[category.Key.ToLower()]
                : null;
            var subcategories = category.Value;
            foreach (var subcategory in subcategories)
            {
                var binds = subcategory.Value;
                Separator commandSeparator = Instantiate(bindSeparatorPrefab, targetTransform).GetComponent<Separator>();
                commandSeparator.SetName($"{category.Key.ToUpper()} - {subcategory.Key}");
                foreach (var bind in binds)
                {
                    BindPanel bindPanel = Instantiate(bindPanelPrefab, targetTransform).GetComponent<BindPanel>();
                    bindPanel.SetBind(bind);
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
                { "controls", controlsPanelContainer },
                { "hidden", hiddenPanelContainer },
            };

    }

    private void ExportBinds()
    {
        // Create a .cfg file
        string filePath = Path.Combine(Application.dataPath, "binds.cfg");

        // Get the biggest command to align the comments
        int maxLeftWidth = bindList.binds
            .Select(bind => $"bind {bind.name} {bind.scancode}".Length)
            .Max();

        var groupedBinds = bindList.binds
            .GroupBy(bind => bind.category)
            .ToDictionary(
                group => group.Key,
                group => group
                    .GroupBy(bind => bind.subcategory)
                    .ToDictionary(subgroup => subgroup.Key, subgroup => subgroup.ToList())
            );


        //CommandList audioCommands = (CommandList)commandList.commands.GroupBy(cmd => cmd.category);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var categoryPair in groupedBinds)
            {
                var subcategories = categoryPair.Value;
                foreach (var subcategory in subcategories)
                {
                    writer.WriteLine();
                    writer.WriteLine($"========== {categoryPair.Key.ToUpper()} - {subcategory.Key} ==========");

                    var binds = subcategory.Value;
                    foreach (var bind in binds)
                    {
                        if (bind.scancode == null)
                            continue;
                        string left = $"bind {bind.name} {bind.scancode}".PadRight(maxLeftWidth + 4);
                        string line = left + $"// {bind.americanKey} -> {bind.ingameName}";
                        writer.WriteLine(line);
                    }
                }
            }
        }
    }
}
