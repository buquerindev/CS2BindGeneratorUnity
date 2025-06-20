using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeyBindInputField : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public delegate void KeyDownDelegate(KeyControl key, Bind bind);
    public static event KeyDownDelegate OnKeyPressed;

    [SerializeField] private BindPanel bindPanel;
    [SerializeField] private TMP_InputField inputField;

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
                // Key name
                rawKey = key.name;

                // Key display (it depends of keyboard layout)
                inputField.text = $"{key.displayName} <color=#88888888>({key.name})</color>";

                // Invoke event so the relative key knows it's pressed
                OnKeyPressed?.Invoke(key, bindPanel.GetBind());
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
        } else if (Mouse.current.forwardButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE4");
        } else if (Mouse.current.backButton.wasPressedThisFrame)
        {
            SetMouseKey("MOUSE5");
        }
    }

    private void Start()
    {

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

    public void LockBind(string name)
    {
        inputField.text = name;
        inputField.interactable = false;
    }

    public void Unbind()
    {
        inputField.text = "";
    }
}
