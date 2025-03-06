using System.Collections.Generic;
using UnityEngine;

public class CrewManager : MonoBehaviour
{
    public static CrewManager Instance { get; private set; }

    [Header("Crew List")]
    public List<CrewmateData> crewList = new List<CrewmateData>();

    [Header("Crew Requests")]
    public List<CrewRequest> activeRequests = new List<CrewRequest>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RecruitCrewmate(CrewmateData newCrewmate)
    {
        if (!crewList.Contains(newCrewmate))
        {
            crewList.Add(newCrewmate);
            newCrewmate.ApplyEffect();
            Debug.Log($"Recruited {newCrewmate.crewmateName}!");
        }
        else
        {
            Debug.Log($"{newCrewmate.crewmateName} is already in the crew!");
        }
    }

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
