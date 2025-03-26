using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CircleStatusIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float maxValue = 100f;
    
    [Header("Tooltip Settings")] 
    [SerializeField] private TooltipPanel tooltipPanel;
    [SerializeField] private float tooltipDelay = 1f;
    [SerializeField, TextArea(2, 5)] private string tooltipDescription;

    private float currentValue;
    private float hoverTime;
    private bool isHovering;
    
    private void Start()
    {
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Vertical;
            fillImage.fillOrigin = (int)Image.Origin360.Bottom;
        }

        if (valueText != null)
        {
            valueText.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (isHovering)
        {
            hoverTime += Time.deltaTime;
            if (hoverTime >= tooltipDelay)
            {
                // Get the position of this status indicator in screen space
                RectTransform rect = (RectTransform)transform;
                Vector2 position = rect.TransformPoint(rect.rect.center);
                tooltipPanel.ShowTooltip(tooltipDescription, position);
            }
        }
    }
    
    public void UpdateFillAmount(float value)
    {
        currentValue = value;
        
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(currentValue / maxValue);
        }

        if (valueText != null)
        {
            valueText.text = Mathf.RoundToInt(value).ToString();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        hoverTime = 0f;
        
        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }

        if (valueText != null)
        {
            valueText.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        hoverTime = 0f;
        
        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(true);
        }

        if (valueText != null)
        {
            valueText.gameObject.SetActive(false);
        }

        if (tooltipPanel != null)
        {
            tooltipPanel.HideTooltip();
        }
    }
}
