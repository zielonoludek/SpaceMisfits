using UnityEngine;
using UnityEngine.UI;

public class RemoveCrewMoraleButton : MonoBehaviour
{
    public Button crewMoraleButton;

    private void Start()
    {
        crewMoraleButton.onClick.AddListener(RemoveCrewMorale);
    }

    private void RemoveCrewMorale()
    {
        ResourceManager.Instance.CrewMorale -= 600;
    }
}
