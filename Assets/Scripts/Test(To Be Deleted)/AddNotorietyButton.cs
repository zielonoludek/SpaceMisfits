using UnityEngine;
using UnityEngine.UI;

public class AddNotorietyButton : MonoBehaviour
{
    public Button notorietyButton;

    private void Start()
    {
        notorietyButton.onClick.AddListener(AddNotoriety);
    }

    private void AddNotoriety()
    {
        ResourceManager.Instance.Notoriety += 500;
    }
}
