using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TimePanelUI timePanelUI;
    [SerializeField] FightPanelUI fightPanelUI;

    public TimePanelUI TimePanelUI { get { return timePanelUI; } }
    public FightPanelUI FightPanelUI { get { return fightPanelUI; } }
}