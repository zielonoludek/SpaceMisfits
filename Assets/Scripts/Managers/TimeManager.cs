using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] int oneDayInSeconds = 600; //10 min
    [SerializeField] int day = 1;
    [SerializeField] int month = 7;
    [SerializeField] int year = 2793;

    public System.Action EndOfDay;

    private bool timeOn = true;
    private int daysCounter;
    private float currentTime;
   
    private float timeSpeed = 1;

    int[] daysInMonths = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    public int TotalDays { get { return daysCounter; } }
    public int Day { get { return day; } }
    public int Month { get { return month; } }
    public int Year { get { return year; } }
    public int DayLength { get { return oneDayInSeconds; } }
    public float CurrentTime { get { return currentTime; } }
    public float TimeSpeed {  get { return timeSpeed; } }

    public bool IsTimeOn { get { return timeOn; } }

    void FixedUpdate()
    {
        if (timeOn)
        {
            currentTime += Time.fixedDeltaTime;
            if (currentTime >= oneDayInSeconds) DayEnded();
        }
    }
    public void SetTimeSpeed(float speed)
    {
        if(speed != 0) timeSpeed = speed;
        Time.timeScale = speed;
    }

    public void PauseTime(bool var)
    {
        if (GameManager.Instance.GameState == GameState.Event) return;
        timeOn = !var;
        if (timeOn) SetTimeSpeed(timeSpeed);
        else SetTimeSpeed(0);
    }
    void DayEnded()
    {
        daysCounter++;
        currentTime = 0;
        if (day < daysInMonths[month - 1]) day++;
        else
        {
            day = 1;
            if (month < 12) month++;
            else month = 1;
        }
        EndOfDay?.Invoke();
    }

    public int GetTimeDifference(Vector3 firstDate, Vector3 secondDate)
    {
        int diff = 0;
        int day1 = (int)firstDate.x;
        int day2 = (int)secondDate.x;
        int month1 = (int)firstDate.y;
        int month2 = (int)secondDate.y;
        int year1 = (int)firstDate.z;
        int year2 = (int)secondDate.z;

        return diff;
    }
    public Vector3 GetFutureDate(int days)
    {
        int currentMonthDays = daysInMonths[Month];
        int d, m, y;
        d = Day; m = Month; y = Year;
        if (Day + days > currentMonthDays)
        {
            int daysInNextMonth = days - (Day - currentMonthDays);
            d = daysInNextMonth;
            if (Month == 12)
            {         
                m = 1;
                y++;
            }
            else m++;
        }
        else d += days;
        return new(d, m, y);
    }
    public Vector2 GetCurrentTime()
    {
        float dayProgress = currentTime / oneDayInSeconds;
        int hours = Mathf.FloorToInt(dayProgress * 24) % 24;
        int minutes = Mathf.FloorToInt((dayProgress * 1440) % 60);

        return new Vector2(hours, minutes);
    }
    public Vector3 GetDate()
    {
        return new Vector3(Day, Month, Year);
    }

}
