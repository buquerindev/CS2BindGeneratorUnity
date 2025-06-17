using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandPanel : MonoBehaviour
{

    private Command command;

    [SerializeField] private TextMeshProUGUI commandName;

    //[SerializeField] private GameObject GOenumDropdown;
    //[SerializeField] private GameObject GOstringDropdown;
    //[SerializeField] private GameObject GOboolToggle;
    //[SerializeField] private GameObject GOintInputField;
    //[SerializeField] private GameObject GOfloatInputField;

    [SerializeField] private TMP_Dropdown enumDropdown;
    [SerializeField] private TMP_Dropdown stringDropdown;
    [SerializeField] private Toggle boolToggle;
    [SerializeField] private TMP_InputField intInputField;
    [SerializeField] private TMP_InputField floatInputField;

    public void SetCommand(Command command)
    {
        this.command = command;
        InitializePanel();
    }

    private void InitializePanel()
    {
        commandName.text = command.ingameName;

        switch (command.type)
        {
            case "bool":
                boolToggle.gameObject.SetActive(true);
                InitializeBoolPanel();
                break;

            case "enum":
                enumDropdown.gameObject.SetActive(true);
                InitializeEnumPanel();
                break;

            case "string":
                stringDropdown.gameObject.SetActive(true);
                InitializeStringPanel();
                break;

            case "int":
                intInputField.gameObject.SetActive(true);
                InitializeIntPanel();
                break;

            case "float":
                floatInputField.gameObject.SetActive(true);
                InitializeFloatPanel();
                break;

            default:
                break;
        }

    }

    private void InitializeBoolPanel()
    {
        if ((bool)command.defaultValue == true)
            boolToggle.isOn = true;
        else
            boolToggle.isOn = false;
    }

    private void InitializeEnumPanel()
    {
        enumDropdown.ClearOptions();
        enumDropdown.AddOptions(command.enumNames);
    }

    private void InitializeStringPanel()
    {
        stringDropdown.ClearOptions();
        stringDropdown.AddOptions(command.optionsNames);
    }

    private void InitializeIntPanel()
    {
        int value = (int)command.defaultValue;
        intInputField.text = value.ToString();
    }

    private void InitializeFloatPanel()
    {
        float value = (float)command.defaultValue;
        floatInputField.text = value.ToString();
    }

}
