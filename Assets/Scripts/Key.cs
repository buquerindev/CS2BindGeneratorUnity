using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.Collections;

public class Key : MonoBehaviour
{
    private Bind bind;

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
    }

    private void OnBind(KeyControl key, Bind bind)
    {
        if(key.name == this.keyName)
        {
            Debug.Log(meshRenderer.material.name);
            if (meshRenderer.material.name == "UnusableKey (Instance)")
                return;
            Select();
            this.bind = bind;
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
