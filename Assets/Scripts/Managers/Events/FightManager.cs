using System.Collections;
using UnityEngine;
using TMPro;

public class FightManager : MonoBehaviour
{
    public TMP_Text fightResultText;
    public TMP_Text rollingEffectText;

    public FightEventSO sampleFightEvent;

    private System.Random random = new System.Random();
    private int playerNotoriety = 100;
    private int playerBooty = 100;
    private int playerFood = 100;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) StartFight(sampleFightEvent);
    }

    public void StartFight(FightEventSO fightEvent)
    {
        int computerBet = Mathf.Min(fightEvent.playerBetNotoriety, fightEvent.GetMaxComputerBet());

        int playerDice = fightEvent.GetMaxPlayerDice(computerBet);
        int computerDice = 1 + computerBet / 10;

        int playerRoll = RollDice(playerDice);
        int computerRoll = RollDice(computerDice);

        StartCoroutine(ShowRollingEffect(playerRoll, computerRoll, fightEvent, computerBet));
    }

    private int RollDice(int diceCount)
    {
        int total = 0;
        for (int i = 0; i < diceCount; i++)
        {
            total += random.Next(1, 7);
        }
        return total;
    }

    private IEnumerator ShowRollingEffect(int playerRoll, int computerRoll, FightEventSO fightEvent, int computerBet)
    {
        int displayPlayer = 0, displayComputer = 0;
        for (int i = 0; i < 20; i++)
        {
            displayPlayer = random.Next(playerRoll / 2, playerRoll + 2);
            displayComputer = random.Next(computerRoll / 2, computerRoll + 2);
            rollingEffectText.text = $"Rolling... Player: {displayPlayer} | Computer: {displayComputer}";
            yield return new WaitForSeconds(0.05f);
        }

        rollingEffectText.text = "";
        ResolveFight(playerRoll, computerRoll, fightEvent, computerBet);
    }

    private void ResolveFight(int playerRoll, int computerRoll, FightEventSO fightEvent, int computerBet)
    {
        if (playerRoll > computerRoll)
        {
            playerNotoriety += computerBet;
            playerBooty += fightEvent.GetBootyWin();
            playerFood += fightEvent.GetFoodWin();
            fightResultText.text = $"<color=green>You won!</color> Gained {computerBet} notoriety, {fightEvent.GetBootyWin()} booty, and {fightEvent.GetFoodWin()} food.";
        }
        else
        {
            playerNotoriety -= fightEvent.playerBetNotoriety;
            playerBooty = Mathf.Max(0, playerBooty - fightEvent.GetBootyLose());
            playerFood = Mathf.Max(0, playerFood - fightEvent.GetFoodLose());
            fightResultText.text = $"<color=red>You lost!</color> Lost {fightEvent.playerBetNotoriety} notoriety, {fightEvent.GetBootyLose()} booty, and {fightEvent.GetFoodLose()} food.";
        }
    }
}
