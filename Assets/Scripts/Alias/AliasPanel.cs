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
    [SerializeField] private Button unbindButton;

    private void Start()
    {
        originalCommandIF.onEndEdit.AddListener(OnOriginalCommandChanged);
        aliasCommandIF.onEndEdit.AddListener(OnAliasCommandChanged);
        unbindButton.onClick.AddListener(() =>
        {
            CommandManager.Instance.aliases.Remove(alias);
            Destroy(gameObject);
        });
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
        AddToAliasList();
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
        AddToAliasList();
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

    private void AddToAliasList()
    {
        if (alias.IsWritten())
        {
            if (!CommandManager.Instance.aliases.Contains(alias))
            {
                CommandManager.Instance.aliases.Add(alias);
            }
        }
    }
}
