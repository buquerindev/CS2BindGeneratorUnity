using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private Transform caretTransform;
    private RectTransform intInputFieldCaretRT;
    [SerializeField] private RectTransform intInputFieldTextRT;

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
        if(caretTransform == null)
            caretTransform = intInputField.textViewport.transform.GetChild(0);
        intInputFieldCaretRT = caretTransform.GetComponent<RectTransform>();

        // We check if the command is int or float, and if the value is valid, else = 0%
        if (command.type == "int")
        {
            if (int.TryParse(value, out int result))
            {
                if(result > (int)command.max)
                {
                    command.selectedValue = command.max;
                    result = (int)command.max;
                } else if (result < (int)command.min)
                {
                    command.selectedValue = command.min;
                    result = (int)command.min;
                }
                else
                {
                    command.selectedValue = result;
                }
                intInputField.text = result.ToString();
            }   
        }
        else
        {
            if (float.TryParse(value, out float fresult))
            {
                if(fresult > (float)command.max)
                {
                    command.selectedValue = command.max;
                    fresult = (float)command.max;
                } else if (fresult < (float)command.min)
                {
                    command.selectedValue = command.min;
                    fresult = (float)command.min;
                } else
                {
                    command.selectedValue = fresult;
                }
                floatInputField.text = fresult.ToString();
            }
        }

        if (CommandManager.Instance.snd_formula_commands.Contains(command.name))
        {
            command.ConvertSoundValue(false);
            intInputField.text = command.selectedValue + "%";
            intInputField.caretPosition = intInputField.text.Length; // Move caret to the end
            intInputFieldTextRT.anchoredPosition = Vector2.zero; // Reset position to avoid caret issues
            intInputFieldCaretRT.anchoredPosition = Vector2.zero; // Reset caret position to avoid issues
        }

        if(CommandManager.Instance.snd_to_decimal_commands.Contains(command.name))
        {
            command.ConvertSoundValue(true);
            intInputField.text = command.selectedValue + "%";
            intInputField.caretPosition = intInputField.text.Length; // Move caret to the end
            intInputFieldTextRT.anchoredPosition = Vector2.zero; // Reset position to avoid caret issues
            intInputFieldCaretRT.anchoredPosition = Vector2.zero; // Reset caret position to avoid issues
        }
            

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
        int value = (int)command.defaultValue;
        enumDropdown.value = command.enumValues.IndexOf(value);
    }

    private void InitializeStringPanel()
    {
        stringDropdown.ClearOptions();
        stringDropdown.AddOptions(command.optionsNames);
        string value = command.defaultValue as string;
        stringDropdown.value = command.options.IndexOf(value);
    }

    private void InitializeIntPanel()
    {
        int value = (int)command.defaultValue;
        intInputField.text = value.ToString();
        CheckSNDCommand(value.ToString());
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

                if (command.type == "enum")
                {
                    enumDropdown.value = command.enumValues.IndexOf(int.Parse(parts[1]));
                    enumDropdown.RefreshShownValue();
                }

                if (command.type == "int")
                {
                    intInputField.text = parts[1].ToString();
                    CheckSNDCommand(parts[1]);
                }

                if (command.type == "float")
                {
                    floatInputField.text = parts[1].ToString();
                }

                if (command.type == "string")
                {
                    stringDropdown.value = command.options.IndexOf(parts[1]);
                    stringDropdown.RefreshShownValue();
                }

                if (command.type == "bool")
                {
                    boolToggle.isOn = bool.Parse(parts[1]);
                }
            }       
        }
    }

    private void CheckSNDCommand(string value)
    {
        if (CommandManager.Instance.snd_formula_commands.Contains(command.name))
        {
            command.ConvertSoundValue(false);
            intInputField.text = value + "%";
        }
        if (CommandManager.Instance.snd_to_decimal_commands.Contains(command.name))
        {
            command.ConvertSoundValue(true);
            intInputField.text = value + "%";
        }
    }

    public void OnSelect(BaseEventData baseEventData)
    {
        Debug.Log("Seleccionado panel " + command.ingameName);
        OnPanelSelected?.Invoke(command);
    }
}
