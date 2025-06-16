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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
