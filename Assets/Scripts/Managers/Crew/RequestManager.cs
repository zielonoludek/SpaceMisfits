using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    public List<CrewRequestSO> requestPool = new List<CrewRequestSO>();
    private List<CrewRequestSO> activeRequests = new List<CrewRequestSO>();
    public int requestCap = 4;
    public float checkInterval = 5f; 
    public float negativeModifier = 1.5f; 

    private void Start()
    {
        StartCoroutine(GenerateRequestsPeriodically());
    }

    private IEnumerator GenerateRequestsPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            TryGenerateRequest();
        }
    }

    public void TryGenerateRequest()
    {
        List<CrewMemberType> availableCrewTypes = GameManager.Instance.CrewManager.crewList
            .Select(crewmate => crewmate.crewMemberType)
            .ToList(); CrewRequestSO[] allRequests = Resources.LoadAll<CrewRequestSO>("ScriptableObjects/Crew/Requests");
        List<CrewRequestSO> validRequests = new List<CrewRequestSO>();

        foreach (CrewRequestSO request in allRequests)
        {
            if (request.specialMember == CrewMemberType.None || availableCrewTypes.Contains(request.specialMember))
            {
                validRequests.Add(request);
            }
        }

        if (validRequests.Count == 0) return;

        CrewRequestSO selectedRequest = validRequests[UnityEngine.Random.Range(0, validRequests.Count)];
        activeRequests.Add(selectedRequest);
        Debug.Log($"New Request Generated: {selectedRequest.Name}");
    }

    private void GenerateRequest(RequestType type)
    {
        List<CrewRequestSO> availableRequests = requestPool.FindAll(r => r.Type == type && !activeRequests.Contains(r));
        if (availableRequests.Count == 0) return;

        CrewRequestSO newRequest = availableRequests[UnityEngine.Random.Range(0, availableRequests.Count)];
        activeRequests.Add(newRequest);

        Debug.Log($"New Request Generated: {newRequest.Name}");
    }

    public void FulfillRequest(CrewRequestSO request)
    {
        if (activeRequests.Contains(request))
        {
            activeRequests.Remove(request);
            IncreaseCrewMood();
            Debug.Log($"Request Fulfilled: {request.Name}");
        }
    }

    private void IncreaseCrewMood()
    {
        Debug.Log("Crew Mood Increased!");
    }

    private void DecreaseCrewMood()
    {
        Debug.Log("Crew Mood Decreased!");
    }

    private void Update()
    {
        CheckExpiredRequests();
    }

    private void CheckExpiredRequests()
    {
        float currentTime = Time.time;
        for (int i = activeRequests.Count - 1; i >= 0; i--)
        {
            CrewRequestSO request = activeRequests[i];
            if (currentTime - request.ExpirationTime > request.ExpirationTime)
            {
                FailRequest(request);
            }
        }
    }

    private void FailRequest(CrewRequestSO request)
    {
        if (activeRequests.Contains(request))
        {
            activeRequests.Remove(request);
            DecreaseCrewMood();
            Debug.Log($"Request Failed: {request.Name}");
        }
    }
    public List<CrewRequestSO> GetActiveRequests()
    {
        return activeRequests;
    }

}