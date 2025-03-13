using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    private int requestCap = 6;
    private float checkInterval = 0;

    [SerializeField] private List<CrewRequestSO> activeRequests = new List<CrewRequestSO>();

    [SerializeField] private Vector3 checkIntervalDaysHoursMinutes = new Vector3(0, 0, 1);
    [SerializeField] private float negativeModifier = 1.5f;

    private void Start()
    {
        StartCoroutine(GenerateRequestsPeriodically());
    }
    private void OnValidate()
    {
        checkInterval = GameManager.Instance?.TimeManager?.ConvertTimeVec3ToSeconds(checkIntervalDaysHoursMinutes) ?? 300;
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
        float probability = CalculateRequestProbability();
        if (Random.value > probability)
        {
            Debug.Log("Generating request failed - probability");
            return;
        }

        List<CrewMemberType> availableCrewTypes = GameManager.Instance.CrewManager.crewList
            .Select(crewmate => crewmate.crewMemberType)
            .ToList();
        CrewRequestSO[] allRequests = Resources.LoadAll<CrewRequestSO>("ScriptableObjects/Crew/Requests");
        List<CrewRequestSO> validRequests = allRequests
            .Where(request => (request.specialMember == CrewMemberType.None || availableCrewTypes.Contains(request.specialMember)) &&
                              !IsRequestAlreadyActive(request))
            .ToList();

        if (validRequests.Count == 0)
        {
            Debug.Log("Generating request failed - no aviable requests");
            return;
        }

        CrewRequestSO selectedRequest = validRequests[Random.Range(0, validRequests.Count)];

        GenerateRequest(selectedRequest);
    }

    public void GenerateRequest(CrewRequestSO request)
    {
        if (IsRequestAlreadyActive(request)) return;

        activeRequests.Add(request);
        request.StartTime = GameManager.Instance.TimeManager.CurrentTime;
        request.ExpirationTime = request.StartTime + request.TimeLimitInSeconds();

        GameManager.Instance.UIManager.CrewRequestUI.AssignRequestButton(request);
    }


    public void FulfillRequest(CrewRequestSO request)
    {
        if (activeRequests.Contains(request) && request.CanFulfillRequest())
        {
            activeRequests.Remove(request);
            
            ApplyEffects(request, true);
            GameManager.Instance.UIManager.CrewRequestUI.CloseRequestPanel();

            GameManager.Instance.UIManager.CrewRequestUI.UpdateRequestButtons();
            Debug.Log($"Request {request.Name} completed. UI updated.");

        }
    }

    private void FailRequest(CrewRequestSO request)
    {
        if (activeRequests.Contains(request))
        {
            activeRequests.Remove(request);

            ApplyEffects(request, false);

            GameManager.Instance.UIManager.CrewRequestUI.UpdateRequestButtons();
            Debug.Log($"Request {request.Name} completed. UI updated.");

        }
    }
    private void ApplyEffects(CrewRequestSO request, bool isReward)
    {
        var effects = isReward ? request.Rewards : request.Penalties;

        foreach (var effect in effects)
        {
            int effectValue = effect.Value;

            switch (effect.Key)
            {
                case EffectType.Booty:
                    GameManager.Instance.ResourceManager.Booty += effectValue;
                    break;
                case EffectType.Notoriety:
                    GameManager.Instance.ResourceManager.Notoriety += effectValue;
                    break;
                case EffectType.Health:
                    GameManager.Instance.ResourceManager.ShipHealth += effectValue;
                    break;
                case EffectType.Sight:
                    GameManager.Instance.ResourceManager.Sight += effectValue;
                    break;
                case EffectType.Speed:
                    GameManager.Instance.ResourceManager.Speed += effectValue;
                        break;
                case EffectType.Food:
                    GameManager.Instance.ResourceManager.Food += effectValue;
                    break;
                case EffectType.CrewMood:
                    GameManager.Instance.ResourceManager.CrewMood += effectValue;
                    break;
                case EffectType.CrewMemberSpot:
                    break;
                case EffectType.Durability:
                    break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) TryGenerateRequest();
        CheckExpiredRequests();
    }

    private void CheckExpiredRequests()
    {
        float currentTime = Time.time;
        for (int i = activeRequests.Count - 1; i >= 0; i--)
        {
            CrewRequestSO request = activeRequests[i];
            if (request.ExpirationTime <= 0f) return;
            if (currentTime > request.ExpirationTime)
            {
                FailRequest(request);
            }
        }
    }

    public List<CrewRequestSO> GetActiveRequests()
    {
        return activeRequests;
    }
    private float CalculateRequestProbability()
    {
        float baseChance = 0.1f;
        int freeSlots = requestCap - activeRequests.Count;
        int maxSlots = requestCap;
        float crewMood = Mathf.Clamp(GameManager.Instance.ResourceManager.CrewMood / 100f, 0f, 1f);

        float slotModifier = 1 + ((float)freeSlots / maxSlots);
        float moodModifier = 1 + (1 - crewMood);

        float probability = baseChance * slotModifier * moodModifier;

        if (crewMood < 0.5f) probability *= negativeModifier;

        return Mathf.Clamp(probability, 0f, 1f);

    }
    public bool IsRequestAlreadyActive(CrewRequestSO request)
    {
        return activeRequests.Contains(request);
    }
}
