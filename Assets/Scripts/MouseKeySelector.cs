using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class MouseKeySelector : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.sKey.isPressed)
        {
            SelectKeyWithMouse();
        }
    }

    private void SelectKeyWithMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            hit.transform.GetComponent<Key>().Select();
        }
    }
}
