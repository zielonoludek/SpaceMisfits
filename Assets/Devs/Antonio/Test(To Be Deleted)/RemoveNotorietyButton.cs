using UnityEngine;
using UnityEngine.UI;

public class RemoveNotorietyButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AddNotoriety);
    }

    private void AddNotoriety()
    {
        GameManager.Instance.ResourceManager.Notoriety -= 500;
    }
}
