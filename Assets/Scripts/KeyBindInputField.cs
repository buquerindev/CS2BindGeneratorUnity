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
    public static event KeyDownDelegate OnKeyPressed;
    public static event KeyDownDelegate OnUnbindKey;

    //public delegate void MouseKeyDownDelegate(string mouseKey, Bind bind);
    //public static event MouseKeyDownDelegate OnMouseKeyPressed;

    [SerializeField] private BindPanel bindPanel;
    [SerializeField] private TMP_InputField inputField;

    private KeyControl lastKeyControl = new();
    private string lastMouseKey;

    private string bindName;

    private bool isListening = false;
    private string rawKey;
    private void Update()
    {
        if (!isListening)
            return;

        foreach (var key in Keyboard.current.allKeys)
        {
            if (key.wasPressedThisFrame)
            {
                // Save the key to unbind later
                lastKeyControl = key;

                // Key display (it depends of keyboard layout)
                inputField.text = $"{key.displayName} <color=#88888888>({key.name})</color>";

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

    public void LoadBind(string localKey, string americanKey, string scancode)
    {
        if (localKey.StartsWith("MOUSE")  || localKey.StartsWith("MWHEEL"))
        {
            inputField.text = $"{localKey}";
            bindPanel.GetBind().SetMouseKey(localKey);
            bindPanel.ActivateUnbindAndExtraKey(true);
            return;
        }

        bindPanel.ActivateUnbindAndExtraKey(true);

        inputField.text = $"{localKey} <color=#88888888>({americanKey})</color>";

        KeyControl key = Keyboard.current.allKeys.FirstOrDefault(k => k.name == americanKey);

        lastKeyControl = key;

        if (key != null)
        {
            OnKeyPressed?.Invoke(key, bindPanel.GetBind());
        }
    }

    public void StartListening()
    {
        StartCoroutine(ListenNextFrame());
    }

    IEnumerator ListenNextFrame()
    {
        yield return null; // espera un frame
        isListening = true;
    }

    private void SetMouseKey(string key)
    {
        lastMouseKey = key;
        inputField.text = key;
        isListening = false;
        bindPanel.GetBind().SetMouseKey(key);
        bindPanel.ActivateUnbindAndExtraKey(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void UnbindMouseKey(string key)
    {
        bindPanel.GetBind().UnbindMouseKey(key);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (isListening)
            return;
        StartListening();
        inputField.text = "";
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isListening = false;
    }

    public void LockBind(string name)
    {
        inputField.text = name;
        inputField.interactable = false;
    }

    public void Unbind()
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
