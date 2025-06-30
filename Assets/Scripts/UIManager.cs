using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public Button shopButton;
    public GameObject restartPanel;
    public Button restartButton;
    public GameObject relicPanel;
    private WaveManager _waveManager;

    private void Start()
    {
        shopButton.onClick.AddListener(OnShop);
        restartButton.onClick.AddListener(OnRestart);

        // Subscribe to events
        GameManager.Instance.OnCoinsUpdated += UpdateCoins;

        // Initial UI update
        UpdateCoins(GameManager.Instance.coins);
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent errors when the object is destroyed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCoinsUpdated -= UpdateCoins;
        }
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

    public void ShowShopButton()
    {
        shopButton.gameObject.SetActive(true);
    }

    public void ShowRelicSelectionPanel()
    {
        relicPanel.gameObject.SetActive(true);
    }
}
