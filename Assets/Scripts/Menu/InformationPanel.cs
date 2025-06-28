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

    string ConvertAsterisksToBold(string input)
    {
        return Regex.Replace(input, @"\*(.*?)\*", "<b>$1</b>");
    }
}
