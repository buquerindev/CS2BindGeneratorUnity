using TMPro;
using UnityEngine;

public class BindInfoPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI americanKey;
    [SerializeField] private TextMeshProUGUI scancode;
    [SerializeField] private TextMeshProUGUI actions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MouseKeySelector.OnKeySelected += UpdatePanelInfo;
        Key.OnKeyDown += UpdatePanelInfo;
    }

    private void UpdatePanelInfo(Key selectedKey)
    {
        if (!MenuManager.Instance.bindsMenuActive)
            return;
        Debug.Log("Actualizando info");
        americanKey.text = selectedKey.GetKeyName();
        scancode.text = selectedKey.GetScanCode();
        actions.text = selectedKey.GetKeyActions();
    }
}
