using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TimePanelUI : MonoBehaviour
{
    [SerializeField] TMP_Text timeTxt;
    [SerializeField] TMP_Text dateTxt;

    [SerializeField] Button pauseResumeBtn;
    [SerializeField] Button speed1xBtn;
    [SerializeField] Button speed2xBtn;

    void Start()
    {
        pauseResumeBtn.onClick.RemoveAllListeners();
        speed1xBtn.onClick.RemoveAllListeners();
        speed2xBtn.onClick.RemoveAllListeners();

        speed1xBtn.onClick.AddListener(() =>
        {
            if (!GameManager.Instance.TimeManager.IsTimeOn) OnPauseBtn();
            GameManager.Instance.TimeManager.SetTimeSpeed(1);
        });
        speed2xBtn.onClick.AddListener(() =>
        {
            if (!GameManager.Instance.TimeManager.IsTimeOn) OnPauseBtn();
            GameManager.Instance.TimeManager.SetTimeSpeed(2);
        });
        
        pauseResumeBtn.onClick.AddListener(OnPauseBtn);

        GameManager.Instance.TimeManager.EndOfDay += SetupDate;

        Setup();
    }
    public void Setup()
    {
        pauseResumeBtn.gameObject.SetActive(true);
        speed1xBtn.gameObject.SetActive(true);
        speed2xBtn.gameObject.SetActive(true);

        timeTxt.gameObject.SetActive(true);
        dateTxt.gameObject.SetActive(true);

        SetupDate();
    }
    void Update()
    {
        SetupTime();
    }
    public void SetupDate()
    {
        Vector3 date = GameManager.Instance.TimeManager.GetDate();
        int day = Mathf.FloorToInt(date.x);
        int month = Mathf.FloorToInt(date.y);
        int year = Mathf.FloorToInt(date.z);

        dateTxt.text = $"{day:D2}.{month:D2}.{year}";
    }
    void SetupTime()
    {
        Vector2 time = GameManager.Instance.TimeManager.GetCurrentTime();
        int hours = Mathf.FloorToInt(time.x);
        int minutes = Mathf.FloorToInt(time.y);

        timeTxt.text = $"{hours:D2}:{minutes:D2}";
    }
    public void SetupPauseBtn()
    {
        if (GameManager.Instance.TimeManager.IsTimeOn) pauseResumeBtn.GetComponentInChildren<TMP_Text>().text = "||";
        else pauseResumeBtn.GetComponentInChildren<TMP_Text>().text = ">";
    }
    private void OnPauseBtn()
    {
        bool isTimePaused = GameManager.Instance.TimeManager.IsTimeOn;
        GameManager.Instance.TimeManager.PauseTime(isTimePaused);
        SetupPauseBtn();
    }

    public void SetTimeSpeed(float speed)
    {
        if(!GameManager.Instance.TimeManager.IsTimeOn) OnPauseBtn();
        GameManager.Instance.TimeManager.SetTimeSpeed(speed);
    }
}
