using UnityEngine;
using System.Collections;
using System;


public class CrewMemberConsumption : MonoBehaviour
{
    private CrewDropZone crewDropZone;
    private CrewmateData crewmateData;
    private GameManager gameManager;
    private string shipPart;
    private string crewName;
    
    [SerializeField] public int currentHunger;
    [SerializeField] public int maxHunger;
    [SerializeField] int addedHunger;
    [SerializeField] int decreaseHunger;

    [SerializeField] public int currentSleepiness;
    [SerializeField] public int maxSleepiness;
    [SerializeField] int addedSleepiness;
    [SerializeField] int sleepinessDecrease;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crewDropZone = transform.parent.GetComponent<CrewDropZone>();
        shipPart = crewDropZone.shipPart.partName;

        crewmateData = GetComponent<CrewMemberDraggable>().crewmateData;
        crewName = crewmateData.crewmateName;

        float hourTime = GameManager.Instance.TimeManager.DayLength / 24f;
        StartCoroutine(ManageHungerAndSleepiness(hourTime));
    }

    // Update is called once per frame
    void Update()
    {

    }

     private IEnumerator ManageHungerAndSleepiness(float hourTime)
        {
        while (true)
        {
            // Any room
                crewDropZone = transform.parent.GetComponent<CrewDropZone>();
                shipPart = crewDropZone.shipPart.partName;
                
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

            // Kitchen
            if (crewDropZone.GetPartName() == "Kitchen")
            {
                while (currentHunger > 0)
                {
                    // Simulate consuming food

                    if (GameManager.Instance.ResourceManager.Food > 0)
                    {
                        currentHunger -= decreaseHunger;
                        GameManager.Instance.ResourceManager.Food -= decreaseHunger;
                    }
                    else
                    {
                        // If no food is available, stop the coroutine or handle accordingly
                        Debug.Log($"{crewName} has no food to consume!");
                        break;
                    }

                    currentHunger -= decreaseHunger;
                    GameManager.Instance.ResourceManager.Food += (decreaseHunger * -1) ;

                    if (currentHunger < 0)
                    {
                        currentHunger = 0;
                    }
                    yield return new WaitForSeconds(GameManager.Instance.TimeManager.DayLength / 24f);
                }
            }

            // Crew Quarters
            if (crewDropZone.GetPartName() == "Crew Quarters")
            {
                while (currentSleepiness > 0)
                {
                    currentSleepiness -= sleepinessDecrease;
                    if (currentSleepiness < 0)
                    {
                        currentSleepiness = 0;
                    }
                    yield return new WaitForSeconds(GameManager.Instance.TimeManager.DayLength / 24f);
                }
            }

            Debug.Log($"{crewName} assigned to: {shipPart}: Hunger: {currentHunger}, Sleepiness: {currentSleepiness}");
            yield return new WaitForSeconds(GameManager.Instance.TimeManager.DayLength / 24f);
            

        
        }
    }
    void OnDestroy()
    {
       
      
    }
}









