using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bindName;
    [SerializeField] private KeyBindInputField keyInputField;
    [SerializeField] private Button unbindButton;

    private Bind bind;

    private void Start()
    {
        unbindButton.onClick.AddListener(keyInputField.Unbind);
    }

    public void SetBind(Bind bind)
    {
        this.bind = bind;
        if (bind.ingameName == "Fire")
        {
            keyInputField.LockBind("MOUSE1");
        }
        bindName.text = bind.ingameName;

        LoadBind();
    }

    public Bind GetBind()
    {
        return bind;
    }

    private void LoadBind()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "binds.txt");
        if (!File.Exists(filePath))
            return;

        string[] lines = File.ReadAllLines(filePath);

        //+jump,Barra Espaciadora,space,scancode44
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(',');
            if (parts.Length != 4)
                continue;

            string name = parts[0];
            
            if (name == bind.name)
            {
                string localKey = parts[1];
                string americanKey = parts[2];
                string scancode = parts[3];

                bind.localKey = localKey;
                bind.americanKey = americanKey;
                bind.scancode = scancode;

                keyInputField.LoadBind(localKey, americanKey, scancode);

                return;
            }
        }
    }
}
