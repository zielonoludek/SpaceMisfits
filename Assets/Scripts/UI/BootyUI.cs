using UnityEngine;
using TMPro;

public class BootyUI : MonoBehaviour
{
    public TextMeshProUGUI bootyText;

    private void Start()
    {
        ResourceManager.Instance.OnBootyChanged += UpdateUI;
        UpdateUI(ResourceManager.Instance.Booty);
    }

    private void OnDestroy()
    {
        ResourceManager.Instance.OnBootyChanged -= UpdateUI;
    }

    private void UpdateUI(int amount)
    {
        bootyText.text = "Booty: " + amount;
    }
}