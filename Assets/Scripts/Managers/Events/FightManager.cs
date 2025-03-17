using System.Collections;
using System.Linq;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    private FightPanelUI fightPanelUI;
    private FightEventSO fightEvent;

    private System.Random random = new System.Random();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) StartFight();
    }

    private void Start()
    {
        fightPanelUI = GameManager.Instance.UIManager.FightPanelUI;
        CloseFight();
    }

    public void StartFight()
    {
        if (fightEvent == null)
        {
            fightEvent = GetRandomFightEvent();
        }
        fightPanelUI.gameObject.SetActive(true);
        fightPanelUI.Setup();

        GameManager.Instance.TimeManager.PauseTime(true);
        GameManager.Instance.GameState = GameState.Event;
        GameManager.Instance.UIManager.TimePanelUI.SetupPauseBtn();

        GameManager.Instance.UIManager.TimePanelUI.SetTimeSpeed(1);
    }

    public void StartFight(FightEventSO fight)
    {
        fightEvent = fight;
        fightPanelUI.gameObject.SetActive(true);

        fightPanelUI.Setup(); GameManager.Instance.TimeManager.PauseTime(true);
        GameManager.Instance.GameState = GameState.Event;
        GameManager.Instance.UIManager.TimePanelUI.SetupPauseBtn();
        
        GameManager.Instance.UIManager.TimePanelUI.SetTimeSpeed(1);
    }

    public void CloseFight()
    {
        fightEvent = null;
        fightPanelUI.gameObject.SetActive(false);

        GameManager.Instance.GameState = GameState.None;
        GameManager.Instance.TimeManager.PauseTime(false);
        GameManager.Instance.UIManager.TimePanelUI.SetupPauseBtn();
    }
    public void SetupFight(int betAmount)
    {
        fightEvent.playerBetNotoriety = betAmount;
        int computerBet = Mathf.Min(fightEvent.playerBetNotoriety, fightEvent.GetMaxComputerBet());

        int computerDiceCount = 1 + computerBet / 10;
        int playerDiceCount = Mathf.Min(fightEvent.GetMaxPlayerDice(computerBet), computerDiceCount + 3);

        int[] playerRolls = RollDice(playerDiceCount);
        int[] computerRolls = RollDice(computerDiceCount);

        int playerTotal = SumRolls(playerRolls);
        int computerTotal = SumRolls(computerRolls);

        StartCoroutine(fightPanelUI.ShowRollingEffect(playerRolls, computerRolls));
        StartCoroutine(ResolveFight(playerTotal, computerTotal, computerBet, betAmount));
    }

    private FightEventSO GetRandomFightEvent()
    {
        FightEventSO[] allEvents = Resources.LoadAll<FightEventSO>("ScriptableObjects/Events/Fights");

        if (allEvents.Length == 0)
        {
            Debug.LogWarning("Brak event�w w folderze 'ScriptableObjects/Events/Fights'!");
            return null;
        }

        return allEvents[random.Next(allEvents.Length)];
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
        return rolls.Sum();
    }

    private IEnumerator ResolveFight(int playerRoll, int computerRoll, int computerBet, int playerBet)
    {
        yield return new WaitForSeconds(1f);

        int notorietyChange, bootyChange, foodChange;
        bool playerWon = playerRoll > computerRoll;
        
        if (playerWon)
        {
            notorietyChange = computerBet;
            bootyChange = fightEvent.GetBootyWin();
            foodChange = fightEvent.GetFoodWin();

            if (playerBet == 0) notorietyChange = fightEvent.GetNotorietyBet0();
            else notorietyChange = computerBet;

            if (fightEvent.crewmateToRecruit != null)
            {
                GameManager.Instance.CrewManager.RecruitCrewmate(fightEvent.crewmateToRecruit);
            }
        }
        else
        {
            notorietyChange = -playerBet;
            bootyChange = -fightEvent.GetBootyLose();
            foodChange = -fightEvent.GetFoodLose();
        }

        GameManager.Instance.ResourceManager.Notoriety += notorietyChange;
        GameManager.Instance.ResourceManager.Booty = Mathf.Max(0, GameManager.Instance.ResourceManager.Booty + bootyChange);
        GameManager.Instance.ResourceManager.Food = Mathf.Max(0, GameManager.Instance.ResourceManager.Food + foodChange);

        fightPanelUI.ShowFightResult(playerWon);
        fightPanelUI.UpdateResourceUI(notorietyChange, bootyChange, foodChange);
    }
}
