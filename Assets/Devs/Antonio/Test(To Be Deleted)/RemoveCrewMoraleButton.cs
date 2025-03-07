using UnityEngine;
using UnityEngine.UI;

public class RemoveCrewMoodButton : MonoBehaviour
{
    public Button CrewMoodButton;

    private void Start()
    {
        CrewMoodButton.onClick.AddListener(RemoveCrewMood);
    }

    private void RemoveCrewMood()
    {
        GameManager.Instance.ResourceManager.CrewMood -= 600;
    }
}
