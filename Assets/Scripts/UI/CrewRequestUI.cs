using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrewRequestUI : MonoBehaviour
{
    private CrewRequestSO crewRequest;

    [SerializeField] private GameObject requestPanel;
    [SerializeField] private List<Button> requestBtns = new List<Button>();
    [SerializeField] private Button fulfillBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Image crewImg;
    [SerializeField] private TMP_Text crewNameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text rewardTxt;
    [SerializeField] private TMP_Text penaltyTxt;
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private TMP_Text requirementTxt;

    public void Start()
    {
        CloseRequestPanel();
    }
    public void Setup()
    {
        closeBtn.onClick.RemoveAllListeners();
        fulfillBtn.onClick.RemoveAllListeners();
        foreach (Button b in requestBtns)
        {
            b.onClick.RemoveAllListeners();
            b.gameObject.SetActive(false);
        }

        fulfillBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.RequestManager.FulfillRequest(crewRequest);
            CloseRequestPanel(); 
        }); 
        closeBtn.onClick.AddListener(() =>
        {
            Debug.Log("cl");
            CloseRequestPanel();
        });
    }
    public void SetupRequestPanel(CrewRequestSO request)
    {
        requestPanel.SetActive(true);
        crewRequest = request;
        UpdateRequestUI();
    }
    public void CloseRequestPanel()
    {
        Debug.Log("ose");
        requestPanel.SetActive(false);
    }

    public void AssignRequestButton(CrewRequestSO request)
    {
        Button freeButton = requestBtns.FirstOrDefault(b => !b.gameObject.activeSelf);

        if (freeButton != null)
        {
            freeButton.gameObject.SetActive(true);
            freeButton.GetComponentInChildren<TMP_Text>().text = request.Requirement.ToString();

            freeButton.onClick.RemoveAllListeners();
            freeButton.onClick.AddListener(() => SetupRequestPanel(request));
        }
    }

    public void UpdateRequestButtons()
    {
        foreach (Button b in requestBtns)
        {
            b.gameObject.SetActive(false);
        }
        foreach (var request in GameManager.Instance.RequestManager.GetActiveRequests())
        {
            AssignRequestButton(request);
        }
    }

    private void UpdateRequestUI()
    {
        if (crewRequest == null) return;

        crewImg.sprite = crewRequest.Artwork;
        crewNameTxt.text = crewRequest.Name;
        descriptionTxt.text = crewRequest.Description;
        rewardTxt.text = string.Join("", crewRequest.Rewards.Select(r => $"{r.Key}: {r.Value}\n"));
        penaltyTxt.text = string.Join("", crewRequest.Penalties.Select(p => $"{p.Key}: {p.Value}\n"));
        timeTxt.text = $"Time: {FormatTime(crewRequest.TimeLimitDayHoursMinutes)}";

        string requirementText = $"Requirement: {crewRequest.Requirement}";

        if (crewRequest.FulfillmentCondition is IntFulfillmentCondition intCondition) requirementText += $" ({intCondition.RequiredValue})";

        requirementTxt.text = requirementText;
    }

    string FormatTime(Vector3 time)
    {
        List<string> timeParts = new List<string>();

        if (time.x > 0) timeParts.Add($"{(int)time.x} days");
        if (time.y > 0) timeParts.Add($"{(int)time.y} hours");
        if (time.z > 0) timeParts.Add($"{(int)time.z} minutes");

        string formattedTime = timeParts.Count > 0 ? string.Join(", ", timeParts) : "No time remaining";

        return formattedTime;
    }
}
