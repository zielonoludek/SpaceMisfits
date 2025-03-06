using UnityEngine;
using UnityEngine.UI;

public class RemoveNotorietyButton : MonoBehaviour
{
    public Button notorietyButton;

    private void Start()
    {
        notorietyButton.onClick.AddListener(AddNotoriety);
    }

    private void AddNotoriety()
    {
        GameManager.Instance.ResourceManager.Notoriety -= 500;
    }
}
