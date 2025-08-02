using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private Button settingsMenuButton;
    [SerializeField] private Button bindsMenuButton;
    [SerializeField] private Button practiceMenuButton;
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private Button switchKeyboard;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject bindsMenu;
    [SerializeField] private GameObject practiceMenu;

    [SerializeField] private GameObject keyboardISO;
    [SerializeField] private GameObject keyboardANSI;

    [SerializeField] private CameraMover cameraMover;
    [SerializeField] private Transform cameraDefault;
    [SerializeField] private Transform cameraZoom;

    [SerializeField] private Toggle unbindallToggle;

    public bool addUnbindall = false;
    public bool isANSI = false;
    public bool bindsMenuActive = false;

    private Vector3 keyboardPosition = new Vector3((float)-43.4039993, (float)7.10900021, (float)13.7150002);
    private Vector3 keyboardRotation = new Vector3((float)357, (float)90, 0);

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 144;
        settingsMenuButton.onClick.AddListener(OpenSettingsMenu);
        bindsMenuButton.onClick.AddListener(OpenBindsMenuButton);
        practiceMenuButton.onClick.AddListener(OpenPracticeMenu);
        closeMenuButton.onClick.AddListener(CloseMenu);
        switchKeyboard.onClick.AddListener(ToggleKeyboard);
        quitButton.onClick.AddListener(Application.Quit);
        unbindallToggle.onValueChanged.AddListener(OnToggleChanged);
        //cameraMover.MoveCameraTo(cameraDefault,1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenSettingsMenu()
    {
        CloseMainMenu();
        settingsMenu.SetActive(true);
    }

    void OpenBindsMenuButton()
    {
        CloseMainMenu();
        bindsMenuActive = true;
        bindsMenu.SetActive(true);
        cameraMover.MoveCameraTo(cameraZoom,2);
    }

    void OpenPracticeMenu()
    {
        CloseMainMenu();
        practiceMenu.SetActive(true);
    }

    void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        closeMenuButton.gameObject.SetActive(false);
    }

    void CloseMainMenu()
    {
        mainMenu.SetActive(false);
        closeMenuButton.gameObject.SetActive(true);
    }

    void CloseMenu()
    {
        bindsMenuActive = false;
        settingsMenu.SetActive(false);
        bindsMenu.SetActive(false);
        practiceMenu.SetActive(false);
        cameraMover.MoveCameraTo(cameraDefault,2);
        OpenMainMenu();
    }

    private void ToggleKeyboard()
    {
        if (!isANSI)
        {
            keyboardISO.transform.position = new Vector3(10,0,0);
            keyboardANSI.transform.position = keyboardPosition;
            isANSI = true;
        }
        else
        {
            keyboardANSI.transform.position = new Vector3(10, 0, 0);
            keyboardISO.transform.position = keyboardPosition;
            isANSI = false;
        }
    }

    private void OnToggleChanged(bool value)
    {
        addUnbindall = value;
    }
}
