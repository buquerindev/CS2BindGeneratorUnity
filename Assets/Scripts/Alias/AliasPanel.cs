using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AliasPanel : MonoBehaviour, ISelectHandler
{
    public delegate void OnSelectHandler(Alias alias);
    public static event OnSelectHandler OnPanelSelected;

    [SerializeField] private TMP_InputField originalCommandIF;
    [SerializeField] private TMP_InputField aliasCommandIF;
    [SerializeField] private Alias alias;

    private void Start()
    {
        originalCommandIF.onEndEdit.AddListener(OnOriginalCommandChanged);
        aliasCommandIF.onEndEdit.AddListener(OnAliasCommandChanged);
        gameObject.AddComponent<Selectable>();
    }

    private void OnOriginalCommandChanged(string newCommand)
    {
        if (string.IsNullOrWhiteSpace(newCommand))
        {
            Debug.LogWarning("Original command cannot be empty.");
            return;
        }

        alias.originalCommand = newCommand;
        OnPanelSelected?.Invoke(alias);
    }

    private void OnAliasCommandChanged(string newAlias)
    {
        if (string.IsNullOrWhiteSpace(newAlias))
        {
            Debug.LogWarning("Original command cannot be empty.");
            return;
        }

        alias.aliasCommand = newAlias;
        OnPanelSelected?.Invoke(alias);
    }

    public void Initialize(string originalCommand = "", string aliasCommand = "")
    {
        alias = new Alias()
        {
            originalCommand = originalCommand,
            aliasCommand = aliasCommand
        };
        CommandManager.Instance.aliases.Add(alias);

        originalCommandIF.text = originalCommand;
        aliasCommandIF.text = aliasCommand;
    }

    public void OnSelect(BaseEventData baseEventData)
    {
        OnPanelSelected?.Invoke(alias);
    }
}
