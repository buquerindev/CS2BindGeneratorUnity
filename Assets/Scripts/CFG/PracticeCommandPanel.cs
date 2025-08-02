using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PracticeCommandPanel : MonoBehaviour, ISelectHandler
{
    public delegate void OnSelectHandler(PracticeCommand command);
    public static event OnSelectHandler OnPanelSelected;

    [SerializeField] private TextMeshProUGUI commandNameText;
    [SerializeField] private TMP_InputField intInputField;
    [SerializeField] private TMP_InputField floatInputField;
    [SerializeField] private Toggle boolToggle;

    private PracticeCommand command;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.AddComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(PracticeCommand commandArg)
    {
        command = commandArg;
        commandNameText.text = command.ingameName;

        if (command.type == "bool")
        {
            boolToggle.gameObject.SetActive(true);

            if ((bool)command.defaultValue == true)
                boolToggle.isOn = true;
            else
                boolToggle.isOn = false;
        }
        else if (command.type == "float")
        {
            floatInputField.gameObject.SetActive(true);
            floatInputField.text = (string)command.selectedValue;
        }
        else if (command.type == "int")
        {
            intInputField.gameObject.SetActive(true);
            intInputField.text = (string)command.selectedValue;
        }
    }




    public void OnSelect(BaseEventData baseEventData)
    {
        Debug.Log("Seleccionado panel " + command.commandName);
        OnPanelSelected?.Invoke(command);
    }
}
