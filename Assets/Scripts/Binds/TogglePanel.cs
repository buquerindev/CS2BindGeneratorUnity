using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TogglePanel : BindPanel
{
    [SerializeField] private ValueInputField valuesInputField;
    [SerializeField] private TMP_InputField valuesTMP;

    public override void Start()
    {
        unbindButton.onClick.AddListener(valuesInputField.Unbind);
        valuesTMP.onEndEdit.AddListener(SendValuesToBind);
        gameObject.AddComponent<Selectable>();
    }

    public override void SetBind(Bind bind)
    {
        this.bind = bind;
        bindName.text = bind.ingameName;
    }

    public override void ActivateUnbindAndExtraKey(bool state)
    {
        unbindButton.gameObject.SetActive(state);
    }

    public override void LoadBind(string[] lines)
    {
        //+jump,Barra Espaciadora,space,scancode44
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length != 5)
                continue;

            string name = parts[0];

            if (name == bind.name)
            {
                string localKey = parts[1];
                string americanKey = parts[2];
                string values = parts[4];
                valuesTMP.text = values;
                valuesInputField.LoadBind(localKey, americanKey);
            }
        }
    }

    private void SendValuesToBind(string values)
    {
        bind.values = values;
        Debug.Log($"Asignado el siguiente bindeo: bind {bind.scancode} toggle {bind.name} {bind.values}");
    }
}
