using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CrewMemberUIBars : MonoBehaviour
{
    [SerializeField] public Image hungerFillImage;
    [SerializeField] public Image sleepinessFillImage;
    [SerializeField] public CrewMemberConsumption consumption;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    public void SetFill(float hunger, float sleepiness)
    {
        float normalizedHunger = Mathf.Clamp01(hunger / consumption.maxHunger);
        float normalizedSleepiness = Mathf.Clamp01(sleepiness / consumption.maxSleepiness);

        hungerFillImage.fillAmount = normalizedHunger;
        sleepinessFillImage.fillAmount = normalizedSleepiness;

    }

    // Update is called once per frame
    void Update()
    {

        SetFill(consumption.currentHunger, consumption.currentSleepiness);
    }

    
}
