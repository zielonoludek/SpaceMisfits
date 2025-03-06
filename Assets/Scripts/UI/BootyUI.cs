using UnityEngine;
using TMPro;

public class BootyUI : MonoBehaviour
{
    public TextMeshProUGUI bootyText;
    public TextMeshProUGUI notorietyText;

    private void Start()
    {
        GameManager.Instance.ResourceManager.OnBootyChanged += UpdateBootyUI;
        GameManager.Instance.ResourceManager.OnNotorietyChanged += UpdateNotorietyUI;
        UpdateBootyUI(GameManager.Instance.ResourceManager.Booty);
        UpdateNotorietyUI(GameManager.Instance.ResourceManager.Notoriety);
    }

    private void OnDestroy()
    {
        GameManager.Instance.ResourceManager.OnBootyChanged -= UpdateBootyUI;
        GameManager.Instance.ResourceManager.OnNotorietyChanged -= UpdateNotorietyUI;
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