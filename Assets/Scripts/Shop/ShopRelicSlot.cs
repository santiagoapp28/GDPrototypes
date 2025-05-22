using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRelicSlot : MonoBehaviour
{
    public Button buyButton;
    public Image relicImage;
    public TextMeshProUGUI relicNameText;
    public TextMeshProUGUI relicDescriptionText;
    public TextMeshProUGUI priceText;

    public Relic currentRelic;
    public int relicPrice;

    public Action<Relic> OnBuyRelic;

    public void Setup(Relic relicData)
    {
        currentRelic = relicData;
        relicImage.sprite = relicData.image;
        relicNameText.text = relicData.relicName;
        relicDescriptionText.text = relicData.description;
        relicImage.color = RelicManager.Instance.rarityColor[(int)relicData.rarity];
        relicPrice = relicData.shopPrice;
        if(priceText != null)
            priceText.text = relicData.shopPrice.ToString("F0");
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => TryBuyCard());
    }

    private void TryBuyCard()
    {
        if (GameManager.Instance.coins >= relicPrice)
        {
            GameManager.Instance.UpdateCoins(-relicPrice);
            AudioManager.Instance.PlaySFX(Sounds.UIClick);
            RelicManager.Instance.AddRelic(currentRelic);
            buyButton.interactable = false; // Prevent double buy}

            OnBuyRelic?.Invoke(currentRelic);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
