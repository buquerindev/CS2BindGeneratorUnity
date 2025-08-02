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
        PracticeCommandPanel.OnPanelSelected += UpdateCommandInformation;
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
        title.text = "Alias";
        if (alias.aliasCommand == "kill_entities")
        {
            command.text = $"alias \"{alias.originalCommand}\" \"{alias.aliasCommand}\"";
            description.text = alias.description;
            return; // Ignore this
        }
            

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

    private void UpdateCommandInformation(PracticeCommand command)
    {
        title.text = command.ingameName;
        this.command.text = command.commandName;
        description.text = ConvertAsterisksToBold(command.description);
    }

    string ConvertAsterisksToBold(string input)
    {
        return Regex.Replace(input, @"\*(.*?)\*", "<b>$1</b>");
    }
}
