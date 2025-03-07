using UnityEngine;
using UnityEngine.UI;

public class AddCrewMoodButton : MonoBehaviour
{
    public Button CrewMoodButton;

    private void Start()
    {
        CrewMoodButton.onClick.AddListener(AddCrewMood);
    }

    private void AddCrewMood()
    {
        GameManager.Instance.ResourceManager.CrewMood += 600;
    }
}
