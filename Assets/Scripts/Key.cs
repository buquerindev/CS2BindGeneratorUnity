using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.Collections;

public class Key : MonoBehaviour
{
    [SerializeField] private string scanCode;
    [SerializeField] private string keyName;

    private void OnEnable()
    {
        KeyboardKeySelector.OnKeyPressed += OnKeyPressed;
    }

    private void OnKeyPressed(KeyControl key)
    {
        if(key.name == this.keyName)
        {
            Debug.Log(key.name + " == " + this.keyName);
            Select();
        }
    }

    public void Select()
    {

    }

}
