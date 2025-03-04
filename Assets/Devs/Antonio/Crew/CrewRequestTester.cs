using UnityEngine;
using UnityEngine.UI;
using System;

public class CrewRequestTester : MonoBehaviour
{
    public Button addRequestButton;
    public Button progressRequestButton;
    public Button completeRequestButton;

    private CrewRequest testRequest;

    private void Start()
    {
        addRequestButton.onClick.AddListener(AddTestRequest);
        progressRequestButton.onClick.AddListener(ProgressTestRequest);
        completeRequestButton.onClick.AddListener(CompleteTestRequest);
    }

    private void AddTestRequest()
    {
        testRequest = ScriptableObject.CreateInstance<CrewRequest>();
        testRequest.requestName = "Find Hidden Treasure";
        testRequest.difficultyLevel = 5;
        testRequest.fulfillmentCondition = "Collect 3 Maps";
        testRequest.requiredAmount = 3;
        testRequest.currentProgress = 0;
        testRequest.expirationDate = DateTime.Now.AddMinutes(1);

        CrewManager.Instance.AddRequest(testRequest);
        Debug.Log("Added Test Request: " + testRequest.requestName);
    }

    private void ProgressTestRequest()
    {
        if (testRequest != null)
        {
            CrewManager.Instance.UpdateRequestProgress(testRequest.requestName, 1);
            Debug.Log("Progressed Request: " + testRequest.requestName);
        }
    }

    private void CompleteTestRequest()
    {
        if (testRequest != null)
        {
            CrewManager.Instance.UpdateRequestProgress(testRequest.requestName, testRequest.requiredAmount);
            Debug.Log("Completed Request: " + testRequest.requestName);
        }
    }
}
