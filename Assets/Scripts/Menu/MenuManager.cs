using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Button settingsMenuButton;
    [SerializeField] private Button bindsMenuButton;
    [SerializeField] private Button closeMenuButton;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject bindsMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingsMenuButton.onClick.AddListener(OpenSettingsMenu);
        bindsMenuButton.onClick.AddListener(OpenBindsMenuButton);
        closeMenuButton.onClick.AddListener(CloseMenu);
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
        bindsMenu.SetActive(true);
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
        settingsMenu.SetActive(false);
        bindsMenu.SetActive(false);
        OpenMainMenu();
    }
}
