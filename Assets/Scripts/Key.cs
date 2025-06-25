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
        KeyBindInputField.OnUnbindKey += OnUnbind;
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
            if (meshRenderer.material.name == "UnusableKey (Instance)")
                return;
            Select();
            binds.Add(bind);
            if (bind.scancode == null)
                bind.scancode = scanCode;
            else bind.secondScancode = scanCode;
                bind.americanKey = keyName;
            bind.localKey = key.displayName;
            Debug.Log($"Asignado {bind.ingameName} a la tecla {key.displayName} ({key.name})");
        }
    }

    private void OnUnbind(KeyControl key, Bind bind)
    {
        if(key.name == this.keyName)
        {
            if (meshRenderer.material.name == "UnusableKey (Instance)")
                return;
            Deselect();
            binds.RemoveAll(b => b.name == bind.name);
            bind.scancode = null;
            bind.americanKey = null;
            bind.localKey = null;
            Debug.Log($"Eliminado el bind {bind.ingameName} de la tecla {key.displayName} ({key.name})");
            Debug.Log($"Ahora hay {binds.Count} bindeos en {this.keyName}");
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

    private void Deselect()
    {
        previousMaterial = defaultMaterial;
        meshRenderer.material = defaultMaterial;
    }

}
