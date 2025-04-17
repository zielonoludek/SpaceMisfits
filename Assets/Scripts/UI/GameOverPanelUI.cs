using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverPanelUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public Button restartButton;
    public Button mainMenuButton;
    public Button damageShipButton;
    public ResourceManager resourceManager;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        damageShipButton.onClick.AddListener(DamageShip);
        resourceManager = GameManager.Instance.ResourceManager;
        resourceManager.OnShipHealthChanged += CheckShipHealth;
    }

    private void CheckShipHealth(int health)
    {
        if (health <= 0)
        {
            ShowGameOver();
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        GameManager.Instance.SetGameOver(true);
    }

    private void RestartGame()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        GameManager.Instance.SetGameOver(false);

        GameManager.Instance.Reset();
        GameManager.Instance.SceneLoader.LoadNewScene(1);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.SceneLoader.LoadNewScene(0);
    }

    private void DamageShip()
    {
        resourceManager.ShipHealth -= 10;
    }

    private void OnDestroy()
    {
        if (resourceManager != null)
            resourceManager.OnShipHealthChanged -= CheckShipHealth;
    }
}