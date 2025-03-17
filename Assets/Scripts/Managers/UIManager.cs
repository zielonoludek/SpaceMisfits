using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TimePanelUI timePanelUI;
    [SerializeField] FightPanelUI fightPanelUI;
    [SerializeField] CrewRequestUI crewRequestUI;

    public TimePanelUI TimePanelUI { get { return timePanelUI; } }
    public FightPanelUI FightPanelUI { get { return fightPanelUI; } }
    public CrewRequestUI CrewRequestUI { get { return crewRequestUI; } }
}