using UnityEngine;
using UnityEngine.UI;

public class AddCrewMoraleButton : MonoBehaviour
{
    public Button crewMoraleButton;

    private void Start()
    {
        crewMoraleButton.onClick.AddListener(AddCrewMorale);
    }

    private void AddCrewMorale()
    {
        ResourceManager.Instance.CrewMorale += 600;
    }
}
