using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public Button shopButton;
    public GameObject restartPanel;
    public Button restartButton;
    private WaveManager _waveManager;

    public TextMeshProUGUI healthText;
    public Image healthBarImage;

    private void Start()
    {
        shopButton.onClick.AddListener(OnShop);
        restartButton.onClick.AddListener(OnRestart);
        UpdateCoins(GameManager.Instance.coins);
        GameManager.Instance.OnCoinsUpdated += UpdateCoins;
    }

    void OnShop()
    {
        GameManager.Instance.GetComponent<StageManager>().GoToShop();
    }

    void OnRestart()
    {
        GameManager.Instance.GetComponent<StageManager>().GoToMenu();
    }

    public void GameOverPanel()
    {
        restartPanel.SetActive(true);
    }

    public void UpdateCoins(int coins)
    {
        coinsText.text = coins.ToString();
    }

    public void UpdateHealth(int health, int maxHealth)
    {
        healthText.text = health + " / " + maxHealth;
        healthBarImage.fillAmount = (float)health / maxHealth;
    }

    public void ShowShopButton()
    {
        shopButton.gameObject.SetActive(true);
    }
}
