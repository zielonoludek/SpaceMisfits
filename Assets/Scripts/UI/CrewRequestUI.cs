using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CrewRequestUI : MonoBehaviour
{
    public GameObject requestPanelPrefab;
    public Transform requestListParent;
    private RequestManager requestManager;

    private void Start()
    {
        requestManager = GameManager.Instance.RequestManager;
        UpdateUI();
        InvokeRepeating(nameof(UpdateUI), 1f, 1f);
    }

    public void UpdateUI()
    {
        foreach (Transform child in requestListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (CrewRequest request in requestManager.GetActiveRequests())
        {
            GameObject panel = Instantiate(requestPanelPrefab, requestListParent);
            panel.transform.Find("RequestName").GetComponent<TMP_Text>().text = request.Name;
            panel.transform.Find("Condition").GetComponent<TMP_Text>().text = request.FulfillmentCondition;
            //panel.transform.Find("Expiration").GetComponent<TMP_Text>().text = GetTimeRemaining(request.ExpirationTime, requestManager.GetRequestCreationTime(request));
        }
    }

    private string GetTimeRemaining(float expirationTime, float creationTime)
    {
        float remaining = (creationTime + expirationTime) - Time.time;
        if (remaining <= 0) return "Expired";
        TimeSpan time = TimeSpan.FromSeconds(remaining);
        return $"{time.Hours}h {time.Minutes}m {time.Seconds}s left";
    }
}