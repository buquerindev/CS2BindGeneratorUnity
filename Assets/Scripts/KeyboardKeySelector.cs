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
        if (Keyboard.current.upArrowKey.isPressed)
        {
            Debug.Log("Se está presionando la flecha hacia arriba");
        }

        
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

public class InputDebugger : MonoBehaviour
{
    void Update()
    {
        foreach (var device in InputSystem.devices)
        {
            foreach (var control in device.allControls)
            {
                if (control is ButtonControl btn && btn.isPressed)
                {
                    Debug.Log($"[Button] {device.displayName}: {control.name} PRESSED");
                }

                if (control is AxisControl axis && Mathf.Abs(axis.ReadValue()) > 0.1f)
                {
                    Debug.Log($"[Axis] {device.displayName}: {control.name} = {axis.ReadValue()}");
                }
            }
        }
    }
}

