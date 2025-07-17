using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InformationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI command;
    [SerializeField] private TextMeshProUGUI description;

    private void Start()
    {
        BindPanel.OnPanelSelected += UpdateBindInformation;
        CommandPanel.OnPanelSelected += UpdateCommandInformation;
        AliasPanel.OnPanelSelected += UpdateAliasInformation;
    }

    private void UpdateBindInformation(Bind bind)
    {
        title.text = bind.ingameName;
        command.text = bind.name;
        description.text = ConvertAsterisksToBold(bind.description);
    }

    private void UpdateCommandInformation(Command command)
    {
        title.text = command.ingameName;
        this.command.text = command.name;
        description.text = ConvertAsterisksToBold(command.description);
    }

    private void UpdateAliasInformation(Alias alias)
    {
        Debug.Log("Se llama a UpdateAliasInformation");
        title.text = "Alias";
        if (alias.IsWritten())
        {
            command.text = $"alias \"{alias.originalCommand}\" \"{alias.aliasCommand}\"";
            description.text = $"If you type {alias.aliasCommand} it would be like typing {alias.originalCommand}";
        }
        else 
        {
            command.text = "No alias set";
            description.text = alias.description;
        }
    }

    string ConvertAsterisksToBold(string input)
    {
        return Regex.Replace(input, @"\*(.*?)\*", "<b>$1</b>");
    }
}
