using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public Button shopButton;
    private WaveManager _waveManager;

    private void Start()
    {
        if(shopButton) shopButton.onClick.AddListener(OnShop);
        UpdateCoins(GameManager.Instance.coins);
    }

    void OnShop()
    {
        GameManager.Instance.GetComponent<StageManager>().GoToShop();
    }

    public void UpdateCoins(int coins)
    {
        coinsText.text = coins.ToString();
    }

    public void ShowShopButton()
    {
        shopButton.gameObject.SetActive(true);
    }
}
