using System.Collections;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] private FightPanelUI fightPanelUI;
    [SerializeField] private FightEventSO fightEvent;

    private System.Random random = new System.Random();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) StartFight();
    }

    private void Start()
    {
        fightPanelUI = GameManager.Instance.UIManager.FightPanelUI;
    }
    public void StartFight()
    {
        fightPanelUI.Setup();
    }
    public void StartFight(FightEventSO fight)
    {
        fightEvent = fight;
        fightPanelUI.Setup();
    }
    public void SetupFight(int betAmount)
    {
        fightEvent.playerBetNotoriety = betAmount;
        int computerBet = Mathf.Min(fightEvent.playerBetNotoriety, fightEvent.GetMaxComputerBet());

        int computerDiceCount = 1 + computerBet / 10;
        int playerDiceCount = Mathf.Min(fightEvent.GetMaxPlayerDice(computerBet), computerDiceCount + 3); // Enforcing the "+3 dice max difference" rule

        int[] playerRolls = RollDice(playerDiceCount);
        int[] computerRolls = RollDice(computerDiceCount);

        int playerTotal = SumRolls(playerRolls);
        int computerTotal = SumRolls(computerRolls);

        StartCoroutine(fightPanelUI.ShowRollingEffect(playerRolls, computerRolls));

        StartCoroutine(ResolveFight(playerTotal, computerTotal, computerBet));
    }

    private int[] RollDice(int diceCount)
    {
        int[] rolls = new int[diceCount];

        for (int i = 0; i < diceCount; i++)
        {
            rolls[i] = random.Next(1, 7);
        }

        return rolls;
    }

    private int SumRolls(int[] rolls)
    {
        int total = 0;
        foreach (int roll in rolls)
        {
            total += roll;
        }
        return total;
    }

    private IEnumerator ResolveFight(int playerRoll, int computerRoll, int computerBet)
    {
        yield return new WaitForSeconds(1f);

        int notorietyChange, bootyChange, foodChange;
        bool playerWon = playerRoll > computerRoll;

        if (playerWon)
        {
            notorietyChange = computerBet;
            bootyChange = fightEvent.GetBootyWin();
            foodChange = fightEvent.GetFoodWin();
        }
        else
        {
            notorietyChange = -fightEvent.playerBetNotoriety;
            bootyChange = -fightEvent.GetBootyLose();
            foodChange = -fightEvent.GetFoodLose();
        }

        ResourceManager.Instance.Notoriety += notorietyChange;
        ResourceManager.Instance.Booty = Mathf.Max(0, ResourceManager.Instance.Booty + bootyChange);
        ResourceManager.Instance.Food = Mathf.Max(0, ResourceManager.Instance.Food + foodChange);

        fightPanelUI.ShowFightResult(playerWon);
        fightPanelUI.UpdateResourceUI(notorietyChange, bootyChange, foodChange);
    }
}
