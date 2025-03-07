using UnityEngine;
using UnityEngine.UI;

public class CrewUI : MonoBehaviour
{
    public Button recruitButton1;
    public Button recruitButton2;
    public Button recruitButton3;

    public CrewmateData crewmate1;
    public CrewmateData crewmate2;
    public CrewmateData crewmate3;

    private void Start()
    {
        recruitButton1.onClick.AddListener(() => GameManager.Instance.CrewManager.RecruitCrewmate(crewmate1));
        recruitButton2.onClick.AddListener(() => GameManager.Instance.CrewManager.RecruitCrewmate(crewmate2));
        recruitButton3.onClick.AddListener(() => GameManager.Instance.CrewManager.RecruitCrewmate(crewmate3));
    }
}
