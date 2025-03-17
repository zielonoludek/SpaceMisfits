using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FightPanelUI : MonoBehaviour
{
    private FightManager fightManager;
    [SerializeField] private Button closeBtn;

    [Header("Panels")]
    [SerializeField] private GameObject bettingPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("Dice UI")]
    [SerializeField] private Transform playerDicesParent;
    [SerializeField] private Transform opponentDicesParent;
    [SerializeField] private GameObject dicePrefab;

    [Header("Betting UI")]
    [SerializeField] private TMP_Text betAmountText;
    [SerializeField] private Button addBetButton;
    [SerializeField] private Button subtractBetButton;
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

        addBetButton.onClick.AddListener(() => ChangeBet(1));
        subtractBetButton.onClick.AddListener(() => ChangeBet(-1));
        fightButton.onClick.AddListener(() => StartFight());
        closeBtn.onClick.AddListener(() => {
            fightManager.CloseFight();
            closeBtn.gameObject.SetActive(false);
        });

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
        betAmount = Mathf.Clamp(betAmount + amount * 10, 0, maxBet);
        UpdateBetUI();
    }

    private void UpdateBetUI()
    {
        betAmountText.text = betAmount.ToString();
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
}
