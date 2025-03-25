using TMPro;
using UnityEngine;

public class TooltipPanel : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private float xOffset = 20f;
    [SerializeField] private float yOffset = 0f;

    private void Start()
    {
        HideTooltip();
    }

    public void ShowTooltip(string text, Vector2 position)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = text;

        // Calculate position to the right of the stat indicator
        Vector2 newPosition = new Vector2(position.x + xOffset, position.y + yOffset);
        tooltipPanel.transform.position = newPosition;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
