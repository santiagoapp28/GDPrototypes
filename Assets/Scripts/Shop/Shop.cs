using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public ShopCardSlot[] slots;
    public Card[] availableCards; // Assign from inspector or fill dynamically
    public TextMeshProUGUI coinsText;

    void Start()
    {
        PopulateShop();
        UpdateCoins(GameManager.Instance.coins);
        GameManager.Instance.OnCoinsUpdated += UpdateCoins;
    }

    void PopulateShop()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < availableCards.Length)
            {
                slots[i].Setup(availableCards[i]);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    public void NextWave()
    {
        FindAnyObjectByType<StageManager>().StartNewStage();
    }

    public void UpdateCoins(int coins)
    {
        coinsText.text = coins.ToString();
    }
}
