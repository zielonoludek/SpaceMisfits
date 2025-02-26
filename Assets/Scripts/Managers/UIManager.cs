using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TimePanelUI timePanelUI;
    
    public TimePanelUI TimePanelUI { get { return timePanelUI; } }
}