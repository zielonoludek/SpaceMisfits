using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FightEventSO", menuName = "Events/New Fight Event")]
public class FightEventSO : EventSO
{
    [Header("Fight-specific settings")]
    public FightTier fightTier;
    public int playerBetNotoriety;

    [Header("Crewmate recruit")] 
    public CrewmateData crewmateToRecruit;
    public int GetMaxComputerBet()
    {
        return fightTier switch
        {
            FightTier.Easy => 30,
            FightTier.Medium => 50,
            FightTier.Hard => 80,
            _ => 30
        };
    }

    public int GetBootyWin() => fightTier switch { FightTier.Easy => 20, FightTier.Medium => 40, FightTier.Hard => 60, _ => 0 };
    public int GetBootyLose() => fightTier switch { FightTier.Easy => 10, FightTier.Medium => 20, FightTier.Hard => 30, _ => 0 };
    public int GetFoodWin() => fightTier switch { FightTier.Easy => 30, FightTier.Medium => 50, FightTier.Hard => 80, _ => 0 };
    public int GetFoodLose() => fightTier switch { FightTier.Easy => 15, FightTier.Medium => 25, FightTier.Hard => 40, _ => 0 };
    public int GetNotorietyBet0() => fightTier switch { FightTier.Easy => 10, FightTier.Medium => 20, FightTier.Hard => 30, _ => 0 };

    public int GetMaxPlayerDice(int computerBet)
    {
        return Mathf.Min(1 + playerBetNotoriety / 10, 3 + computerBet / 10);
    }
}
