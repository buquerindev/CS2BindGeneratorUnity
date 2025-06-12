using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeyboardKeySelector : MonoBehaviour
{
    public delegate void KeyDownDelegate(KeyControl key);
    public static event KeyDownDelegate OnKeyPressed;

    private bool canSelect = false;

    private void Update()
    {
        if (Mouse.current.middleButton.wasReleasedThisFrame)
        {
            canSelect = true;
            Debug.Log("Activado: presiona una tecla física");
        }

        if (canSelect)
        {
            foreach (var key in Keyboard.current.allKeys)
            {
                if (key.wasPressedThisFrame)
                {
                    Debug.Log("Tecla física detectada: " + key.name);
                    OnKeyPressed?.Invoke(key);
                    //canSelect = false;
                    break;
                }
            }
        }
    }
}
