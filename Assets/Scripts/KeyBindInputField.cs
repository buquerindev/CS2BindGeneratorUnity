using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeyBindInputField : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public delegate void KeyDownDelegate(KeyControl key, Bind bind);
    public static Action<KeyControl, Bind> OnKeyPressed;
    public static Action<KeyControl, Bind> OnUnbindKey;

    //public delegate void MouseKeyDownDelegate(string mouseKey, Bind bind);
    //public static event MouseKeyDownDelegate OnMouseKeyPressed;

    [SerializeField] private BindPanel bindPanel;
    [SerializeField] protected TMP_InputField inputField;

    protected KeyControl lastKeyControl = new();
    protected string lastMouseKey;
    protected string lastInputFieldText;

    private string bindName;

    protected bool isListening = false;
    private string rawKey;
    public virtual void Update()
    {
        if (!isListening)
            return;

        foreach (var key in Keyboard.current.allKeys)
        {
            if (key.wasPressedThisFrame)
            {
                // Unbind last key
                OnUnbindKey?.Invoke(lastKeyControl, bindPanel.GetBind());

                // Save the key to unbind later
                lastKeyControl = key;

                // Key display (it depends of keyboard layout)
                inputField.text = $"{key.displayName} <color=#88888888>({key.name})</color>";
                lastInputFieldText = inputField.text;

                // Invoke event so the relative key knows it's pressed
                OnKeyPressed?.Invoke(key, bindPanel.GetBind());
                bindPanel.ActivateUnbindAndExtraKey(true);
                isListening = false;
                EventSystem.current.SetSelectedGameObject(null);
                break;
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE2");
        } else if (Mouse.current.middleButton.isPressed)
        {
            SetMouseKey("MOUSE3");
        } else if (Mouse.current.backButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE4");
        } else if (Mouse.current.forwardButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE5");
        } else
        {

        }

            Vector2 scrollValue = Mouse.current.scroll.ReadValue();

        if (scrollValue.y > 0f)
        {
            SetMouseKey("MWHEELUP");
        }
        else if (scrollValue.y < 0f)
        {
            SetMouseKey("MWHEELDOWN");
        }
    }

    public virtual void LoadBind(string localKey, string americanKey)
    {
        if (localKey.StartsWith("MOUSE")  || localKey.StartsWith("MWHEEL"))
        {
            inputField.text = $"{localKey}";
            bindPanel.GetBind().SetMouseKey(localKey);
            bindPanel.ActivateUnbindAndExtraKey(true);
            lastMouseKey = localKey;
            return;
        }

        bindPanel.ActivateUnbindAndExtraKey(true);

        inputField.text = $"{localKey} <color=#88888888>({americanKey})</color>";
        lastInputFieldText = inputField.text;

        KeyControl key = Keyboard.current.allKeys.FirstOrDefault(k => k.name == americanKey);

        lastKeyControl = key;

        if (key != null)
        {
            OnKeyPressed?.Invoke(key, bindPanel.GetBind());
        }
    }

    protected void StartListening()
    {
        StartCoroutine(ListenNextFrame());
    }

    IEnumerator ListenNextFrame()
    {
        yield return null; // espera un frame
        isListening = true;
    }

    public virtual void SetMouseKey(string key)
    {
        lastMouseKey = key;
        inputField.text = key;
        lastInputFieldText = inputField.text;
        isListening = false;
        bindPanel.GetBind().SetMouseKey(key);
        bindPanel.ActivateUnbindAndExtraKey(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UnbindMouseKey(string key)
    {
        bindPanel.GetBind().UnbindMouseKey(key);
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        if (isListening)
            return;
        StartListening();
        lastInputFieldText = inputField.text;
        inputField.text = "";
        bindPanel.OnSelect(eventData);
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        isListening = false;
        if(inputField.text == "")
            inputField.text = lastInputFieldText;
    }

    public void LockBind(string name)
    {
        inputField.text = name;
        inputField.interactable = false;
    }

    public virtual void Unbind()
    {
        if(inputField.text.StartsWith("MOUSE") || inputField.text.StartsWith("MWHEEL"))
        {
            UnbindMouseKey(lastMouseKey);
        }
        inputField.text = "";
        if (lastKeyControl != null)
        {
            OnUnbindKey?.Invoke(lastKeyControl, bindPanel.GetBind());
        }
        bindPanel.ActivateUnbindAndExtraKey(false);
    }
}
