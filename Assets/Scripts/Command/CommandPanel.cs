using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;
using static UnityEngine.Rendering.DebugUI;

public class CommandPanel : MonoBehaviour, ISelectHandler
{
    public delegate void OnSelectHandler(Command command);
    public static event OnSelectHandler OnPanelSelected;

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

    private void Start()
    {
        gameObject.AddComponent<Selectable>();
    }

    public void SetCommand(Command command)
    {
        this.command = command;
        InitializePanel();
        enumDropdown.onValueChanged.AddListener(DropdownUpdateValue);
        stringDropdown.onValueChanged.AddListener(DropdownUpdateValue);
        boolToggle.onValueChanged.AddListener(ToggleUpdateValue);
        intInputField.onEndEdit.AddListener(InputFieldUpdateValue);
        floatInputField.onEndEdit.AddListener(InputFieldUpdateValue);
    }

    private void DropdownUpdateValue(int value)
    {
        if(command.type == "string")
            command.selectedValue = command.options[value];
        else command.selectedValue = command.enumValues[value];
    }

    private void ToggleUpdateValue(bool value)
    {
        command.selectedValue = value;
    }

    private void InputFieldUpdateValue(string value)
    {
        if (command.type == "int")
            command.selectedValue = int.Parse(value);
        else
        {
            if (value.StartsWith("."))
                value = "0" + value;
            command.selectedValue = value;
        }

        if (command.name.StartsWith("snd_") || command.name == "volume")
        {
            command.ConvertSoundValue(false);
            intInputField.text = value + "%";
            floatInputField.text = value + "%";
        }

        if(command.name == "snd_spatialize_lerp")
            command.ConvertSoundValue(true);

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

    public void LoadCommand(string[] lines)
    {
        //+jump,Barra Espaciadora,space,scancode44
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length != 2)
                continue;

            string name = parts[0];
            
            if(name == command.name)
            {
                command.selectedValue = parts[1];
                floatInputField.text = parts[1].ToString();
                intInputField.text = parts[1].ToString();

                // PONER IF SND_ CALCULAR EL CONVERTED Y AÑADIR %
                if (command.name.StartsWith("snd_") || command.name == "volume")
                {
                    command.ConvertSoundValue(false);
                    floatInputField.text += "%";
                    intInputField.text += "%";
                }
                if (command.name == "snd_spatialize_lerp")
                    command.ConvertSoundValue(true);
            }
                
        }
    }

    public void OnSelect(BaseEventData baseEventData)
    {
        Debug.Log("Seleccionado panel " + command.ingameName);
        OnPanelSelected?.Invoke(command);
    }
}
