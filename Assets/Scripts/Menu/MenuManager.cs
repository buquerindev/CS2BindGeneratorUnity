using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Button settingsMenuButton;
    [SerializeField] private Button bindsMenuButton;
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private Button switchKeyboard;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject bindsMenu;

    [SerializeField] private GameObject informationPanel;

    [SerializeField] private GameObject keyboardISO;
    [SerializeField] private GameObject keyboardANSI;

    [SerializeField] private CameraMover cameraMover;
    [SerializeField] private Transform cameraDefault;
    [SerializeField] private Transform cameraCenter;
    [SerializeField] private Transform cameraZoom;

    private bool whichKeyboard = false;

    private Vector3 keyboardPosition = new Vector3((float)1.294, (float)-0.042, (float)2.287);
    private Vector3 keyboardRotation = new Vector3((float)334.473145, 0, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingsMenuButton.onClick.AddListener(OpenSettingsMenu);
        bindsMenuButton.onClick.AddListener(OpenBindsMenuButton);
        closeMenuButton.onClick.AddListener(CloseMenu);
        switchKeyboard.onClick.AddListener(ToggleKeyboard);
        cameraMover.MoveCameraTo(cameraCenter,2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenSettingsMenu()
    {
        CloseMainMenu();
        settingsMenu.SetActive(true);
        informationPanel.SetActive(true);
    }

    void OpenBindsMenuButton()
    {
        CloseMainMenu();
        bindsMenu.SetActive(true);
        cameraMover.MoveCameraTo(cameraZoom,1);
    }

    void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        closeMenuButton.gameObject.SetActive(false);
    }

    void CloseMainMenu()
    {
        mainMenu.SetActive(false);
        informationPanel.SetActive(true);
        closeMenuButton.gameObject.SetActive(true);
    }

    void CloseMenu()
    {
        settingsMenu.SetActive(false);
        bindsMenu.SetActive(false);
        informationPanel.SetActive(false);
        cameraMover.MoveCameraTo(cameraCenter,1);
        OpenMainMenu();
    }

    private void ToggleKeyboard()
    {
        if (!whichKeyboard)
        {
            keyboardISO.transform.position = new Vector3(10,0,0);
            keyboardANSI.transform.position = keyboardPosition;
            whichKeyboard = true;
        }
        else
        {
            keyboardANSI.transform.position = new Vector3(10, 0, 0);
            keyboardISO.transform.position = keyboardPosition;
            whichKeyboard = false;
        }
    }
}
