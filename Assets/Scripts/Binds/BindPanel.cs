using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindPanel : MonoBehaviour , ISelectHandler
{
    [SerializeField] public TextMeshProUGUI bindName;
    [SerializeField] public KeyBindInputField keyInputField;
    [SerializeField] private KeyBindInputField extraKeyInputField;
    [SerializeField] public Button unbindButton;

    public delegate void OnSelectHandler(Bind bind);
    public static event OnSelectHandler OnPanelSelected;

    public Bind bind;

    private bool loadedFirstKey = false;

    public virtual void Start()
    {
        unbindButton.onClick.AddListener(keyInputField.Unbind);
        unbindButton.onClick.AddListener(extraKeyInputField.Unbind);
        gameObject.AddComponent<Selectable>();
    }

    // Assigns a bind to this panel
    public virtual void SetBind(Bind bind)
    {
        this.bind = bind;
        if (bind.ingameName == "Fire")
        {
            keyInputField.LockBind("MOUSE1");
        }
        bindName.text = bind.ingameName;
    }

    // Returns the bind associated with this panel
    public Bind GetBind()
    {
        return bind;
    }

    // Loads a pre-existing bind from binds.txt
    public virtual void LoadBind(string[] lines)
    {
        //+jump,Barra Espaciadora,space,scancode44
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length != 4)
                continue;

            string name = parts[0];
            
            if (name == bind.name)
            {
                string localKey = parts[1];
                string americanKey = parts[2];
                string scancode = parts[3];

                if(!loadedFirstKey)
                {
                    keyInputField.LoadBind(localKey, americanKey);
                    loadedFirstKey = true;
                } else
                {
                    extraKeyInputField.LoadBind(localKey, americanKey);
                    return;
                }   
            }
        }
    }

    public virtual void ActivateUnbindAndExtraKey(bool state)
    {
        unbindButton.gameObject.SetActive(state);
        extraKeyInputField.gameObject.SetActive(state);
    }

    public void OnSelect(BaseEventData baseEventData)
    {
        Debug.Log("Seleccionado panel " + bind.ingameName);
        OnPanelSelected?.Invoke(bind);
    }

    public void SendSelectEvent()
    {
        
    }
}
