using System;
using UnityEngine;
using TMPro;

using System.Collections.Generic;
using UnityEngine.UI;

public class CrewRequestUI : MonoBehaviour
{
    private CrewRequestSO crewRequest;

    [SerializeField] private GameObject requestPanel;
    [SerializeField] private List<Button> requestBtns = new List<Button>();

    [Space]
    [SerializeField] private Button fulfillBtn;
    [SerializeField] private Image crewImg;
    [SerializeField] private TMP_Text crewNameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text rewardTxt;
    [SerializeField] private TMP_Text penaltyTxt;
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private TMP_Text requrementTxt;
   

    private void Start()
    {
      
    }

    public void Setup(CrewRequestSO request)
    {
        crewRequest = request;
        UpdateUI();
    }
    private void UpdateUI() 
    {
        
    }
}