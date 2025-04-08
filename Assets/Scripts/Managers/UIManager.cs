using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TimePanelUI timePanelUI;
    [SerializeField] FightPanelUI fightPanelUI;
    [SerializeField] CrewRequestUI crewRequestUI;
    [SerializeField] EventPopupUI eventPanelUI;
    [SerializeField] OptionsPanel optionsPanelUI;
    [SerializeField] PausePanelUI pausePanelUI;

    public TimePanelUI TimePanelUI { get { return timePanelUI; } }
    public FightPanelUI FightPanelUI { get { return fightPanelUI; } }
    public CrewRequestUI CrewRequestUI { get { return crewRequestUI; } }
    public EventPopupUI EventPanelUI { get { return eventPanelUI; } }
    public OptionsPanel OptionsPanelUI { get { return optionsPanelUI; } }
    public PausePanelUI PausePanelUI { get { return pausePanelUI; } }
}