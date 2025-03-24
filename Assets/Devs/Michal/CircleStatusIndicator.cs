using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CircleStatusIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float maxValue = 100f;

    private float currentValue;
    
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
        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(true);
        }

        if (valueText != null)
        {
            valueText.gameObject.SetActive(false);
        }
    }
}
