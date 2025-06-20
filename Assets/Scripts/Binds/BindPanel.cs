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

    private Bind bind;

    private void Start()
    {
        unbindButton.onClick.AddListener(keyInputField.Unbind);
    }

    public void SetBind(Bind bind)
    {
        this.bind = bind;
        if (bind.ingameName == "Fire")
        {
            keyInputField.LockBind("MOUSE1");
        }
        bindName.text = bind.ingameName;
    }

    public Bind GetBind()
    {
        return bind;
    }
}
