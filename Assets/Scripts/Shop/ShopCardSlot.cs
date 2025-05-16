using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardSlot : MonoBehaviour
{
    public Button buyButton;
    public CardUI cardUI;
    public TextMeshProUGUI priceText;

    private Card currentCard;

    public void Setup(Card cardData)
    {
        currentCard = cardData;
        cardUI.Initialize(cardData);
        priceText.text = cardData.shopPrice.ToString("F0");
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => TryBuyCard());
    }

    private void TryBuyCard()
    {
        if (GameManager.Instance.coins >= currentCard.shopPrice)
        {
            GameManager.Instance.UpdateCoins(-currentCard.shopPrice);
            AudioManager.Instance.PlaySFX(Sounds.UIClick);
            DeckManager.Instance.AddCard(currentCard);
            buyButton.interactable = false; // Prevent double buy
            gameObject.SetActive(false);
        }
    }
}
