using System;
using TMPro;
using UnityEngine;

public class HoverUI : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        HidePopup();
    }

    public void ShowPopup(string eventHoverInfo, Vector3 worldPosition)
    {
        if (eventHoverInfo != "")
        {
            popupText.text = eventHoverInfo;
        }
        else
        {
            popupText.text = "Unknown info";
        }
        
        popupPanel.SetActive(true);

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        popupPanel.transform.position = screenPosition + new Vector3(50, 50, 0);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}