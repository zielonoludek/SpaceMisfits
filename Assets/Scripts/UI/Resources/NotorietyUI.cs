using UnityEngine;

public class NotorietyUI : MonoBehaviour
{
    [SerializeField] private CircleStatusIndicator statusIndicator;

    private void Start()
    {
        GameManager.Instance.ResourceManager.OnNotorietyChanged += UpdateNotorietyUI;
        UpdateNotorietyUI(GameManager.Instance.ResourceManager.Notoriety);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.ResourceManager != null)
        {
            GameManager.Instance.ResourceManager.OnNotorietyChanged -= UpdateNotorietyUI;
        }
    }

    private void UpdateNotorietyUI(int amount)
    {
        statusIndicator.UpdateFillAmount(amount);
    }
}
