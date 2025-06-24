using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.Collections;
using System.Collections.Generic;

public class Key : MonoBehaviour
{
    private List<Bind> binds;

    [SerializeField] private string scanCode;
    [SerializeField] private string keyName;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material pressingMaterial;

    private Material previousMaterial;

    private MeshRenderer meshRenderer;

    private void OnEnable()
    {
        KeyBindInputField.OnKeyPressed += OnBind;
        KeyboardKeySelector.OnKeyPressed += OnKeyPressed;
        KeyboardKeySelector.OnKeyReleased += OnKeyReleased;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = defaultMaterial;
        binds = new List<Bind>();
    }

    private void OnBind(KeyControl key, Bind bind)
    {
        if(key.name == this.keyName)
        {
            Debug.Log(meshRenderer.material.name);
            if (meshRenderer.material.name == "UnusableKey (Instance)")
                return;
            Select();
            binds.Add(bind);
            bind.scancode = scanCode;
            bind.americanKey = keyName;
            bind.localKey = key.displayName;
            Debug.Log($"Asignado {bind.ingameName} a la tecla {key.displayName} ({key.name})");
        }
    }


    private void OnKeyPressed(KeyControl key)
    {
        if (key.name == this.keyName)
        {
            if (meshRenderer.material.name == "UnusableKey (Instance)")
                return;
            previousMaterial = meshRenderer.material;
            meshRenderer.material = pressingMaterial;
        }
    }

    private void OnKeyReleased(KeyControl key)
    {
        if (key.name == this.keyName)
        {
            if (meshRenderer.material.name == "UnusableKey (Instance)")
                return;
            meshRenderer.material = previousMaterial;
        }
    }

    public void Select()
    {
        previousMaterial = selectedMaterial;
        meshRenderer.material = selectedMaterial;
    }

}
