using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class ValueInputField : KeyBindInputField, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TogglePanel togglePanel;

    public override void Update()
    {
        if (!isListening)
            return;

        var pressedKeys = Keyboard.current.allKeys
        .Where(k => k != null);

        foreach (var key in pressedKeys)
        {
            if (key.wasPressedThisFrame)
            {
                // Save the key to unbind later
                lastKeyControl = key;

                // Key display (it depends of keyboard layout)
                inputField.text = $"{key.displayName} <color=#88888888>({key.name})</color>";
                Debug.Log("El texto deberia cambiar a " + key.displayName);
                lastInputFieldText = inputField.text;

                // Invoke event so the relative key knows it's pressed
                togglePanel.ActivateUnbindAndExtraKey(true);
                KeyBindInputField.OnKeyPressed?.Invoke(key, togglePanel.GetBind());
                isListening = false;
                EventSystem.current.SetSelectedGameObject(null);
                break;
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE2");
        }
        else if (Mouse.current.middleButton.isPressed)
        {
            SetMouseKey("MOUSE3");
        }
        else if (Mouse.current.backButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE4");
        }
        else if (Mouse.current.forwardButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE5");
        }
        else
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

    public override void LoadBind(string localKey, string americanKey)
    {
        if (localKey.StartsWith("MOUSE") || localKey.StartsWith("MWHEEL"))
        {
            inputField.text = $"{localKey}";
            togglePanel.GetBind().SetMouseKey(localKey);
            togglePanel.ActivateUnbindAndExtraKey(true);
            lastMouseKey = localKey;
            return;
        }

        togglePanel.ActivateUnbindAndExtraKey(true);

        inputField.text = $"{localKey} <color=#88888888>({americanKey})</color>";
        lastInputFieldText = inputField.text;

        KeyControl key = Keyboard.current.allKeys.FirstOrDefault(k => k.name == americanKey);
        lastKeyControl = key;

        if (key != null)
        {
            // Usar togglePanel porque la clase hija no necesita ni toca bindPanel
            KeyBindInputField.OnKeyPressed?.Invoke(key, togglePanel.GetBind());
        }
    }

    public override void SetMouseKey(string key)
    {
        lastMouseKey = key;
        inputField.text = key;
        lastInputFieldText = inputField.text;
        isListening = false;
        togglePanel.GetBind().SetMouseKey(key);
        togglePanel.ActivateUnbindAndExtraKey(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public override void Unbind()
    {
        if (inputField.text.StartsWith("MOUSE") || inputField.text.StartsWith("MWHEEL"))
        {
            UnbindMouseKey(lastMouseKey);
        }
        inputField.text = "";
        if (lastKeyControl != null)
        {
            KeyBindInputField.OnUnbindKey?.Invoke(lastKeyControl, togglePanel.GetBind());
        }
        togglePanel.ActivateUnbindAndExtraKey(false);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        isListening = false;
        if (inputField.text == "")
            inputField.text = lastInputFieldText;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (isListening)
            return;
        StartListening();
        lastInputFieldText = inputField.text;
        inputField.text = "";
        togglePanel.OnSelect(eventData);
    }
}
