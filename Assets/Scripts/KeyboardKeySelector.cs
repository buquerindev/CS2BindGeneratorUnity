using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeyboardKeySelector : MonoBehaviour
{
    public delegate void KeyDownDelegate(KeyControl key);
    public static event KeyDownDelegate OnKeyPressed;
    public static event KeyDownDelegate OnKeyReleased;

    private KeyControl pressedKey;
    private KeyControl unreleasedKey;

    private bool canSelect = false;

    

    private void Update()
    {

        var pressedKeys = Keyboard.current.allKeys
        .Where(k => k != null && k.wasPressedThisFrame);
        var unreleasedKeys = Keyboard.current.allKeys
            .Where(k => k != null && k.wasReleasedThisFrame);

        foreach (var key in pressedKeys)
        {
            Debug.Log("Tecla pulsada: " + key.displayName);
            OnKeyPressed?.Invoke(key);
        }

        foreach (var key in unreleasedKeys)
        {
            Debug.Log("Tecla soltada: " + key.displayName);
            OnKeyReleased?.Invoke(key);
        }
    }
}

