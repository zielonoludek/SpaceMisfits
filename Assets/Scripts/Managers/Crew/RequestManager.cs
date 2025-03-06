using System.Collections.Generic;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    private List<CrewRequest> activeRequests = new List<CrewRequest>();

    public void AddRequest(CrewRequest request)
    {
        if (!activeRequests.Contains(request))
        {
            activeRequests.Add(request);
            Debug.Log($"New Crew Request: {request.requestName}");
        }
    }

    public void UpdateRequestProgress(string requestName, int amount)
    {
        foreach (CrewRequest request in activeRequests)
        {
            if (request.requestName == requestName)
            {
                request.currentProgress += amount;
                Debug.Log($"{request.requestName} progress: {request.currentProgress}/{request.requiredAmount}");

                if (request.IsFulfilled())
                {
                    CompleteRequest(request);
                }
                break;
            }
        }
    }

    private void CompleteRequest(CrewRequest request)
    {
        int rewardAmount = request.difficultyLevel * 100;
        GameManager.Instance.ResourceManager.Booty += rewardAmount;
        Debug.Log($"Request '{request.requestName}' completed! Reward: {rewardAmount} Booty.");
        activeRequests.Remove(request);
    }

    public void CheckRequestExpiration()
    {
        activeRequests.RemoveAll(request => request.IsExpired());
    }
}
