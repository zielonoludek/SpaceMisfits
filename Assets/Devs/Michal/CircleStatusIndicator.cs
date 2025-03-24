using System;
using UnityEngine;
using UnityEngine.UI;

public class CircleStatusIndicator : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float maxValue = 100f;

    private void Start()
    {
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Vertical;
            fillImage.fillOrigin = (int)Image.Origin360.Bottom;
        }
    }
    
    public void UpdateFillAmount(float currentValue)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(currentValue / maxValue);
        }
    }
}
