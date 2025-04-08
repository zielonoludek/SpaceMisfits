using System;
using TMPro;
using UnityEngine;

public class BootyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bootyText;

    private void Start()
    {
        GameManager.Instance.ResourceManager.OnBootyChanged += UpdateBootyUI;
        UpdateBootyUI(GameManager.Instance.ResourceManager.Booty);
    }

    private void OnDestroy()
    {
        GameManager.Instance.ResourceManager.OnBootyChanged -= UpdateBootyUI;
    }
    
    private void UpdateBootyUI(int amount)
    {
        bootyText.text = "OO " + amount;
    }
}
