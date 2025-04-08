using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightPanelUI : MonoBehaviour
{
    private FightManager fightManager;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI eventTitleText;
    [SerializeField] private TextMeshProUGUI eventDescriptionText;

    [Header("Panels")]
    [SerializeField] private GameObject bettingPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("Dice UI")]
    [SerializeField] private Transform playerDicesParent;
    [SerializeField] private Transform opponentDicesParent;
    [SerializeField] private GameObject dicePrefab;

    [Header("Betting UI")]
    [SerializeField] private TMP_Text betAmountText;
    [SerializeField] private Button[] betButtons;
    [SerializeField] private Button fightButton;

    [Header("Resources UI")]
    [SerializeField] private TMP_Text notorietyAmountText;
    [SerializeField] private TMP_Text bootyAmountText;
    [SerializeField] private TMP_Text foodAmountText;
    [SerializeField] private TMP_Text fightResultText;

    private int betAmount = 0;

    private void Start()
    {
        fightManager = GameManager.Instance.FightManager;

        int[] betValues = { -100, -50, -10, -5, -1, 1, 5, 10, 50, 100 };
        for (int i = 0; i < betButtons.Length; i++)
        {
            int value = betValues[i];
            betButtons[i].onClick.AddListener(() => ChangeBet(value));
        }

        fightButton.onClick.AddListener(() => StartFight());
        closeBtn.onClick.AddListener(() => {
            fightManager.CloseFight();
            closeBtn.gameObject.SetActive(false);
        });
        
        AddHoverEffectToButtonText(fightButton);
        AddHoverEffectToButtonText(closeBtn);
        AddHoverEffectToButtonText(betButtons);
    }

    public void Setup()
    {
        betAmount = 0;
        ShowBettingPanel();
        UpdateBetUI();
        UpdateResourceUI(0, 0, 0);
    }

    private void ChangeBet(int amount)
    {
        int maxBet = GameManager.Instance.ResourceManager.Notoriety;
        betAmount = Mathf.Clamp(betAmount + amount, 0, maxBet);
        UpdateBetUI();
    }

    private void UpdateBetUI()
    {
        betAmountText.text = "Bet " + betAmount;
        ValidateFightButton();
    }

    private void StartFight()
    {
        if (GameManager.Instance.ResourceManager.Notoriety < 0) return;
        ShowResultPanel();
        fightManager.SetupFight(betAmount);
    }

    public void ShowBettingPanel()
    {
        closeBtn.gameObject.SetActive(true);
        bettingPanel.SetActive(true);
        resultPanel.SetActive(false);
    }

    public void ShowResultPanel()
    {
        closeBtn.gameObject.SetActive(true);
        bettingPanel.SetActive(false);
        resultPanel.SetActive(true);
    }

    public void UpdateResourceUI(int notorietyChange, int bootyChange, int foodChange)
    {
        notorietyAmountText.text = FormatChange(notorietyChange);
        bootyAmountText.text = FormatChange(bootyChange);
        foodAmountText.text = FormatChange(foodChange);
    }

    private string FormatChange(int value)
    {
        if (value == 0) return "0";
        return $"<color={(value > 0 ? "green" : "red")}>{value:+#;-#}</color>";
    }

    private void ValidateFightButton()
    {
        fightButton.interactable = GameManager.Instance.ResourceManager.Notoriety >= 0;
    }

    public void ShowFightResult(bool playerWon)
    {
        fightResultText.text = playerWon ? "<color=green>Winner</color>" : "<color=red>Loser</color>";
    }

    public int GetPlayerDiceCount(int notoriety)
    {
        return notoriety / 10 + 1;
    }

    public int GetComputerDiceCount(int betAmount)
    {
        return Mathf.Max(1, betAmount / 10);
    }

    public IEnumerator ShowRollingEffect(int[] playerRolls, int[] computerRolls)
    {
        ClearDiceUI(playerDicesParent);
        ClearDiceUI(opponentDicesParent);

        GameObject[] playerDiceObjects = new GameObject[playerRolls.Length];
        GameObject[] computerDiceObjects = new GameObject[computerRolls.Length];

        for (int i = 0; i < playerRolls.Length; i++)
            playerDiceObjects[i] = CreateDice(playerDicesParent);

        for (int i = 0; i < computerRolls.Length; i++)
            computerDiceObjects[i] = CreateDice(opponentDicesParent);

        System.Random random = new System.Random();

        for (int i = 0; i < 20; i++)
        {
            foreach (GameObject dice in playerDiceObjects)
                SetDiceValue(dice, random.Next(1, 7));

            foreach (GameObject dice in computerDiceObjects)
                SetDiceValue(dice, random.Next(1, 7));

            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 0; i < playerRolls.Length; i++)
            SetDiceValue(playerDiceObjects[i], playerRolls[i]);

        for (int i = 0; i < computerRolls.Length; i++)
            SetDiceValue(computerDiceObjects[i], computerRolls[i]);
    }

    public void UpdateEventText(FightEventSO fightEvent)
    {
        eventTitleText.text = fightEvent.eventTitle;
        eventDescriptionText.text = fightEvent.eventDescription;
    }

    private void ClearDiceUI(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private GameObject CreateDice(Transform parent)
    {
        return Instantiate(dicePrefab, parent);
    }

    private void SetDiceValue(GameObject dice, int value)
    {
        TMP_Text diceText = dice.GetComponentInChildren<TMP_Text>();
        if (diceText != null)
        {
            diceText.text = value.ToString();
        }
    }
    
    private void AddHoverEffectToButtonText(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener((data) => { buttonText.color = Color.red; });
            trigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExitEntry.callback.AddListener((data) => { buttonText.color = Color.black; });
            trigger.triggers.Add(pointerExitEntry);
        }
    }
    
    private void AddHoverEffectToButtonText(Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            if (button == null) continue;

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null) continue;

            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener((data) => { buttonText.color = Color.red; });
            trigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExitEntry.callback.AddListener((data) => { buttonText.color = Color.black; });
            trigger.triggers.Add(pointerExitEntry);
        }
    }
}
