using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private bool timeOn = true;
    [SerializeField] private int oneDayInSeconds = 600; // 10 min
    [SerializeField] private int day = 1;
    [SerializeField] private int month = 7;
    [SerializeField] private int year = 2793;

    public System.Action EndOfDay;

    private int daysCounter;
    private float currentTime;
    private float totalTime;
    private float timeSpeed = 1;

    private readonly int[] daysInMonths = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    public int TotalDays => daysCounter;
    public int Day => day;
    public int Month => month;
    public int Year => year;
    public int DayLength => oneDayInSeconds;
    public float CurrentTime => currentTime;
    public float TotalTime => currentTime + daysCounter * oneDayInSeconds;
    public float TimeSpeed => timeSpeed;
    public bool IsTimeOn => timeOn;

    void FixedUpdate()
    {
        totalTime = TotalTime;
        if (timeOn)
        {
            currentTime += Time.fixedDeltaTime;
            if (currentTime >= oneDayInSeconds) DayEnded();
        }
    }

    public void SetTimeSpeed(float speed)
    {
        if (speed != 0) timeSpeed = speed;
        Time.timeScale = speed;
    }

    public void PauseTime(bool pause)
    {
        if (GameManager.Instance.GameState == GameState.Event) return;
        timeOn = !pause;
        SetTimeSpeed(timeOn ? timeSpeed : 0);
    }

    public void Reset()
    {
        day = 1;
        month = 7;
        year = 2793;
        currentTime = 0;
        totalTime = 0;
        daysCounter = 0;
        oneDayInSeconds = 600;
    }
    private void DayEnded()
    {
        daysCounter++;
        currentTime = 0;
        if (day < daysInMonths[month - 1])
        {
            day++;
        }
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
        int day1 = (int)firstDate.x, day2 = (int)secondDate.x;
        int month1 = (int)firstDate.y, month2 = (int)secondDate.y;
        int year1 = (int)firstDate.z, year2 = (int)secondDate.z;

        int diff = (year2 - year1) * 365 + (month2 - month1) * 30 + (day2 - day1);
        return Mathf.Abs(diff);
    }

    public Vector3 GetFutureDate(int days)
    {
        int d = day, m = month, y = year;
        int currentMonthDays = daysInMonths[m - 1];

        while (days > 0)
        {
            if (d + days > currentMonthDays)
            {
                days -= (currentMonthDays - d + 1);
                d = 1;
                m = (m == 12) ? 1 : m + 1;
                if (m == 1) y++;
                currentMonthDays = daysInMonths[m - 1];
            }
            else
            {
                d += days;
                days = 0;
            }
        }
        return new Vector3(d, m, y);
    }

    public Vector2 GetCurrentTime()
    {
        float dayProgress = currentTime / oneDayInSeconds;
        int hours = Mathf.FloorToInt(dayProgress * 24);
        int minutes = Mathf.FloorToInt((dayProgress * 1440) % 60);
        return new Vector2(hours, minutes);
    }

    public Vector3 GetDate()
    {
        return new Vector3(day, month, year);
    }
    public float ConvertTimeToFloat(Vector3 time)
    {
        float seconds = (time.x * oneDayInSeconds) + (time.y * (oneDayInSeconds / 24f)) + (time.z * (oneDayInSeconds / 1440f));
        return seconds;
    }
    public Vector3 ConvertFloatToTime(float seconds)
    {
        int days = Mathf.FloorToInt(seconds / oneDayInSeconds);
        seconds %= oneDayInSeconds;

        int hours = Mathf.FloorToInt(seconds / (oneDayInSeconds / 24f));
        seconds %= (oneDayInSeconds / 24f);

        int minutes = Mathf.FloorToInt(seconds / (oneDayInSeconds / 1440f));

        return new Vector3(days, hours, minutes);
    }
}
