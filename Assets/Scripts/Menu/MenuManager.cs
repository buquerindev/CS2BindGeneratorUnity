using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Button settingsMenuButton;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingsMenuButton.onClick.AddListener(OpenSettingsMenuButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenSettingsMenuButton()
    {
        CloseMainMenu();
        settingsMenu.SetActive(true);
    }

    void CloseMainMenu()
    {
        mainMenu.SetActive(false);
    }
}
