using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TimelineManager : MonoBehaviour
{
    [Header("Timeline Settings")]
    public float pixelsPerDay = 100f; // How many pixels equal 1 in-game day
    public float timeWindowDays = 5f; // How many days ahead are visible on the timeline


    [Header("References")]
    public RectTransform content;               // The content area inside the scroll view
    public GameObject timelineEventPrefab;      // Prefab for each event icon
    public Transform eventContainer;            // Parent object for instantiated event icons

    public TimeManager timeManager;
    public SectorManager sectorManager;


    private float currentStartTime;             // Time at the start (left) of the timeline window
    private List<GameObject> activeEventIcons = new List<GameObject>();

    private GameObject playerArrivalIcon;


    void Start()
    {
        timeManager = GameManager.Instance.TimeManager;
        sectorManager = SectorManager.Instance;
        sectorManager.OnETAUpdated += HandleETAUpdated;

        currentStartTime = timeManager.CurrentTime; // Timeline always starts at current time
        UpdateTimelineWidth();

        Debug.Log("TimelineManager initialized with content: " + content.name);
    }

    private void OnDestroy()
    {
        if (sectorManager != null)
        {
            sectorManager.OnETAUpdated -= HandleETAUpdated;
        }
    }
    void Update()
    {
        // Continuously scroll the timeline left as time passes
        float newStartTime = timeManager.CurrentTime;
        float timeDelta = newStartTime - currentStartTime;

        if (Mathf.Abs(timeDelta) > Mathf.Epsilon)
        {
            // Shift content to the left in pixels based on time passed
            float shiftAmount = timeDelta * pixelsPerDay;
            content.anchoredPosition -= new Vector2(shiftAmount, 0);
            currentStartTime = newStartTime;

            UpdateTimelineEvents(); // Refresh event positions
        }
    }

    void UpdateTimelineWidth()
    {
        // Set the total width of the timeline content
        float width = timeWindowDays * pixelsPerDay;
        content.sizeDelta = new Vector2(width, content.sizeDelta.y);
    }

    void UpdateTimelineEvents()
    {
        ClearOldEvents();

        float currentTime = timeManager.CurrentTime;
        float endTime = currentTime + timeWindowDays;
    }

    void AddTimelineEvent(float eventTime, string type)
    {
        GameObject icon = Instantiate(timelineEventPrefab, eventContainer);
        RectTransform rt = icon.GetComponent<RectTransform>();

        float offsetFromStart = (eventTime - currentStartTime) * pixelsPerDay;
        rt.anchoredPosition = new Vector2(offsetFromStart, 0); // y=0 for horizontal layout

        activeEventIcons.Add(icon);

        // You can add different icons later by checking "type"
    }
    void ClearOldEvents()
    {
        foreach (GameObject go in activeEventIcons)
        {
            Destroy(go);
        }
        activeEventIcons.Clear();
    }

    private void HandleETAUpdated(float remaningSeconds)
    {
        float arivalTime = timeManager.CurrentTime + remaningSeconds;

        float startTime = timeManager.CurrentTime;
        float endTime = startTime + timeWindowDays;

        if (arivalTime >= startTime && arivalTime <= endTime)
        {
            UpdatePlayerArivalIcon(arivalTime);
        }
    }

    private void UpdatePlayerArivalIcon(float arivalTime)
    {
        if (playerArrivalIcon == null)
        {
            playerArrivalIcon = Instantiate(timelineEventPrefab, eventContainer);
        }
    RectTransform rt = playerArrivalIcon.GetComponent<RectTransform>();
        float offsetFromStart = (arivalTime - currentStartTime) * pixelsPerDay;
        rt.anchoredPosition = new Vector2(offsetFromStart, 0);
    }
}