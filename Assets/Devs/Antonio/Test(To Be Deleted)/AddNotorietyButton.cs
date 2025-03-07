using UnityEngine;
using UnityEngine.UI;

public class AddNotorietyButton : MonoBehaviour
{
    public Button notorietyButton;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AddNotoriety);
    }

    private void AddNotoriety()
    {
        GameManager.Instance.ResourceManager.Notoriety += 500;
    }
}
