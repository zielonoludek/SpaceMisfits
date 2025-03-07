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

    public Image crewmateImage1;
    public Image crewmateImage2;
    public Image crewmateImage3;

    private void Start()
    {

        crewmateImage1.gameObject.SetActive(false);
        crewmateImage2.gameObject.SetActive(false);
        crewmateImage3.gameObject.SetActive(false);

        recruitButton1.onClick.AddListener(() => RecruitAndShowImage(crewmate1, crewmateImage1));
        recruitButton2.onClick.AddListener(() => RecruitAndShowImage(crewmate2, crewmateImage2));
        recruitButton3.onClick.AddListener(() => RecruitAndShowImage(crewmate3, crewmateImage3));

    }

    private void RecruitAndShowImage(CrewmateData crewmate, Image crewmateImage)
    {
        GameManager.Instance.CrewManager.RecruitCrewmate(crewmate);

        crewmateImage.sprite = crewmate.crewmateImage;
        crewmateImage.gameObject.SetActive(true);
    }

}
