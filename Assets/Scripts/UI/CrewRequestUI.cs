using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AYellowpaper.SerializedCollections;

public class CrewRequestUI : MonoBehaviour
{
    private CrewRequestSO crewRequest;

    [SerializeField] private GameObject requestPanel;
    [SerializeField] private List<Button> requestBtns = new List<Button>();
    [SerializeField] private Button fulfillBtn;
    [SerializeField] private Image crewImg;
    [SerializeField] private TMP_Text crewNameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text rewardTxt;
    [SerializeField] private TMP_Text penaltyTxt;
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private TMP_Text requrementTxt;

    public void Setup(CrewRequestSO request)
    {
        crewRequest = request;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (crewRequest == null) return;

        crewImg.sprite = crewRequest.Artwork;
        crewNameTxt.text = crewRequest.Name;
        descriptionTxt.text = crewRequest.Description;
        rewardTxt.text = string.Join(", ", crewRequest.Rewards.Select(r => $"{r.Key}: {r.Value}"));
        penaltyTxt.text = string.Join(", ", crewRequest.Penalties.Select(p => $"{p.Key}: {p.Value}"));
        timeTxt.text = crewRequest.TimeLimit.ToString();
        requrementTxt.text = crewRequest.Requrement.ToString();
    }
}