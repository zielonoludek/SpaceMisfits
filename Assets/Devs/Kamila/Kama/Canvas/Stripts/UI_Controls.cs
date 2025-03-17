using UnityEngine;

public class UI_Controls : MonoBehaviour
{
    [SerializeField] GameObject ui_Controls;

    public void OpenControls()
    {
        ui_Controls.SetActive(true);
    }
    public void CloseControls()
    {
        ui_Controls.SetActive(false);
    }
}
