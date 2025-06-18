using TMPro;
using UnityEngine;

public class Separator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTMP;
    public void SetName(string name)
    {
        nameTMP.text = name;
    }
}
