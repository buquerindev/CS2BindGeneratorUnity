using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class MouseKeySelector : MonoBehaviour
{
    // Delegate to handle key selection
    public delegate void KeySelectedHandler(Key selectedKey);
    public static event KeySelectedHandler OnKeySelected;
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Mouse button pressed, selecting key...");
            SelectKeyWithMouse();
        }
    }

    private void SelectKeyWithMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            OnKeySelected?.Invoke(hit.collider.gameObject.GetComponent<Key>());
        }
    }
}
