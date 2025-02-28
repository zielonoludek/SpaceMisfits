using UnityEngine;
using TMPro;
using System;

public class CrewRequestUI : MonoBehaviour
{
    public GameObject requestPanelPrefab;
    public Transform requestListParent;

    private void Start()
    {
        UpdateUI();
        InvokeRepeating(nameof(UpdateUI), 1f, 1f);
    }

    public void UpdateUI()
    {
        foreach (Transform child in requestListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (CrewRequest request in CrewManager.Instance.activeRequests)
        {
            GameObject panel = Instantiate(requestPanelPrefab, requestListParent);
            panel.transform.Find("RequestName").GetComponent<TMP_Text>().text = request.requestName;
            panel.transform.Find("Difficulty").GetComponent<TMP_Text>().text = $"Difficulty: {request.difficultyLevel}";
            panel.transform.Find("Condition").GetComponent<TMP_Text>().text = request.fulfillmentCondition;
            panel.transform.Find("Expiration").GetComponent<TMP_Text>().text = GetTimeRemaining(request.expirationDate);
        }
    }
    private string GetTimeRemaining(DateTime expirationDate)
    {
        TimeSpan remaining = expirationDate - DateTime.Now;
        if (remaining.TotalSeconds <= 0) return "Expired";
        return $"{remaining.Hours}h {remaining.Minutes}m {remaining.Seconds}s left";
    }
}
