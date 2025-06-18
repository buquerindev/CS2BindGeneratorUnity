using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bindName;
    [SerializeField] private KeyBindInputField keyInputField;
    [SerializeField] private Button unbindButton;

    private void Start()
    {
        unbindButton.onClick.AddListener(keyInputField.Unbind);
    }

    public void SetBind(string name)
    {
        if (name == "Fire")
        {
            keyInputField.SetName("MOUSE1");
        }
        bindName.text = name;
    }

    

}
