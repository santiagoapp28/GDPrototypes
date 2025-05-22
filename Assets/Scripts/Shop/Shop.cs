using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public ShopCardSlot[] slots;
    public ShopRelicSlot relicSlot;
    public List<Relic> relics = new List<Relic>();
    public List<Card> availableCards = new List<Card>();
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
            int randomIndex = Random.Range(0, availableCards.Count);
            slots[i].Setup(availableCards[randomIndex]);
            availableCards.RemoveAt(randomIndex);
        }

        relicSlot.Setup(relics[Random.Range(0, relics.Count)]);
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
