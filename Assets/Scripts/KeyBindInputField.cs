using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class KeyBindInputField : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_InputField inputField;
    private bool isListening = false;
    private void Update()
    {
        if (!isListening)
            return;

        foreach (var key in Keyboard.current.allKeys)
        {
            if (key.wasPressedThisFrame)
            {
                Debug.Log("Tecla física detectada: " + key.displayName);
                inputField.text = $"{key.displayName} <color=#88888888>({key.name})</color>";

                isListening = false;
                EventSystem.current.SetSelectedGameObject(null);
                break;
            }
        }
        //if (Mouse.current.leftButton.wasPressedThisFrame)
        //{
        //    SetMouseKey("MOUSE1");
        //} else
        //
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE2");
        } else if (Mouse.current.middleButton.isPressed)
        {
            SetMouseKey("MOUSE3");
        } else if (Mouse.current.forwardButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE4");
        } else if (Mouse.current.backButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE5");
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
        Debug.Log("Tecla física detectada: " + key);
        inputField.text = key;
        isListening = false;
        EventSystem.current.SetSelectedGameObject(null);
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

    public void SetName(string name)
    {
        inputField.text = name;
        inputField.interactable = false;
    }

    public void Unbind()
    {
        inputField.text = "";
    }
}
