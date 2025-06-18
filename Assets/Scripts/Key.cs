using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.Collections;

public class Key : MonoBehaviour
{
    [SerializeField] private string scanCode;
    [SerializeField] private string keyName;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial;

    private BindList bindList;

    private MeshRenderer meshRenderer;

    private void OnEnable()
    {
        KeyboardKeySelector.OnKeyPressed += OnKeyPressed;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = defaultMaterial;
    }

    private void OnKeyPressed(KeyControl key)
    {
        if(key.name == this.keyName)
        {
            Debug.Log(key.name);
            Select();
        }
    }

    public void Select()
    {
        meshRenderer.material = selectedMaterial;
    }

}
