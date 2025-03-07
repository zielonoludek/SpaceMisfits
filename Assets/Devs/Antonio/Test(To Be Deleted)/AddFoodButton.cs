using UnityEngine;
using UnityEngine.UI;

public class AddFoodButton : MonoBehaviour
{
    public Button foodButton;

    private void Start()
    {
        foodButton.onClick.AddListener(AddFood);
    }

    private void AddFood()
    {
        GameManager.Instance.ResourceManager.Food += 550;
    }
}
