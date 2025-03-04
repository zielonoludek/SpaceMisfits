using UnityEngine;
using TMPro;

public class BootyUI : MonoBehaviour
{
    public TextMeshProUGUI bootyText;
    public TextMeshProUGUI notorietyText;

    private void Start()
    {
        ResourceManager.Instance.OnBootyChanged += UpdateBootyUI;
        ResourceManager.Instance.OnNotorietyChanged += UpdateNotorietyUI;
        UpdateBootyUI(ResourceManager.Instance.Booty);
        UpdateNotorietyUI(ResourceManager.Instance.Notoriety);
    }

    private void OnDestroy()
    {
        ResourceManager.Instance.OnBootyChanged -= UpdateBootyUI;
        ResourceManager.Instance.OnNotorietyChanged -= UpdateNotorietyUI;
    }

    private void UpdateBootyUI(int amount)
    {
        bootyText.text = "Booty: " + amount;
    }

    private void UpdateNotorietyUI(int amount)
    {
        notorietyText.text = "Notoriety: " + amount;
    }
}