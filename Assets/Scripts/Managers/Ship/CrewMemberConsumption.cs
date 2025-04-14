using UnityEngine;
using System.Collections;
using System;


public class CrewMemberConsumption : MonoBehaviour
{
    [SerializeField] private CrewDropZone crewDropZone;

    private CrewmateData crewmateData;
    private GameManager gameManager;
    private string shipPart;
    private string crewName;

    [SerializeField] public int currentHunger;
    [SerializeField] public int maxHunger;
    [SerializeField] public int addedHunger;
    [SerializeField] public int decreaseHunger;
    [SerializeField] public int hungerThreshold;

    [SerializeField] public int currentSleepiness;
    [SerializeField] public int maxSleepiness;
    [SerializeField] public int addedSleepiness;
    [SerializeField] public int sleepinessDecrease;
    [SerializeField] public int sleepinessThreshold;

    [SerializeField] public CrewRequestSO hungerCrewRequestSO;
    [SerializeField] public CrewRequestSO sleepinessCrewRequestSO;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crewDropZone = transform.parent.GetComponent<CrewDropZone>();
        shipPart = crewDropZone.shipPart.partName;

        crewmateData = GetComponent<CrewMemberDraggable>().crewmateData;
        crewName = crewmateData.crewmateName;

        float hourTime = GameManager.Instance.TimeManager.DayLength / 24f;
        StartCoroutine(ManageHungerAndSleepiness(hourTime));
        StartCoroutine(EvaluateCrewRequests(hourTime));
    }

    // Update is called once per frame
    void Update()
    {

    }

    // manages hunger and sleepiness calculation
    private IEnumerator ManageHungerAndSleepiness(float hourTime)
    {
        while (true)
        {
            crewDropZone = transform.parent.GetComponent<CrewDropZone>();
            shipPart = crewDropZone.shipPart.partName;
            
            // case for Kitchen - this is hardcoded name right now, as crew drop zones don't have real references
            if (crewDropZone.GetPartName() == "Kitchen")
            {
                IncreaseHungerSleepiness(0, addedSleepiness);
                DecreaseHunger();
                yield return new WaitForSeconds(hourTime);
            }

            // Crew Quarters
            if (crewDropZone.GetPartName() == "Crew Quarters")
            {
                IncreaseHungerSleepiness(addedHunger, 0);
                DecreaseSleepiness();
                yield return new WaitForSeconds(hourTime);
            }

            IncreaseHungerSleepiness(addedHunger, addedSleepiness);
            Debug.Log($"{crewName} assigned to: {shipPart}: Hunger: {currentHunger}, Sleepiness: {currentSleepiness}");
            yield return new WaitForSeconds(hourTime);
        }
    }

    private IEnumerator EvaluateCrewRequests(float hourTime)
    {
        while (true)
        {
            if (currentHunger >= hungerThreshold)
            {
                GameManager.Instance.RequestManager.GenerateRequest(hungerCrewRequestSO);
                CrewRequestSO hungerRequest = GameManager.Instance.RequestManager.GetLastGeneratedRequest(hungerCrewRequestSO);
                hungerRequest.IsFromCrewMember = true;
                hungerRequest.CrewMemberName = crewName;
                hungerRequest.originCrewMemberConsumption = this;
            }

            if (currentSleepiness >= sleepinessThreshold)
            {
                GameManager.Instance.RequestManager.GenerateRequest(sleepinessCrewRequestSO);
                CrewRequestSO sleepinessRequest = GameManager.Instance.RequestManager.GetLastGeneratedRequest(sleepinessCrewRequestSO);
                sleepinessRequest.IsFromCrewMember = true;
                sleepinessRequest.CrewMemberName = crewName;
                sleepinessCrewRequestSO.originCrewMemberConsumption = this;
            }

            yield return new WaitForSeconds(hourTime);
        }
    }

    void IncreaseHungerSleepiness(int addedHunger, int addedSleepiness)
    {
        // common for all rooms
        currentHunger += addedHunger;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

        currentSleepiness += addedSleepiness;
        if (currentSleepiness > maxSleepiness)
        {
            currentSleepiness = maxSleepiness;
        }
    }

    void DecreaseHunger()
    {
        if (GameManager.Instance.ResourceManager.Food > 0)
        {
            currentHunger -= decreaseHunger;
            GameManager.Instance.ResourceManager.Food -= decreaseHunger;
        }
        else
        {
            // If no food is available, go back to increasing hunger
            //currentHunger += addedHunger;
            Debug.Log($"{crewName} has no food to consume!");
        }

        if (currentHunger < 0)
        {
            currentHunger = 0;
        }
    }

    void DecreaseSleepiness()
    {
        currentSleepiness -= sleepinessDecrease;
        if (currentSleepiness < 0)
        {
            currentSleepiness = 0;
        }
    }

    void OnDestroy()
    {

    }
}











