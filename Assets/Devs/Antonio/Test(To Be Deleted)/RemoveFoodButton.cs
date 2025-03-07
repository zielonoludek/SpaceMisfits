using UnityEngine;
using UnityEngine.UI;

public class RemoveFoodButton : MonoBehaviour
{
    public Button foodButton;

    private void Start()
    {
        foodButton.onClick.AddListener(RemoveFood);
    }

    private void RemoveFood()
    {
        GameManager.Instance.ResourceManager.Food -= 550;
    }
}
