using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    [Header("Deck Setup")]
    public int startingHandSize = 5;

    [Header("UI Setup")]
    public Transform handPanel;
    public Transform deckCornerPanel;
    public GameObject cardUIPrefab;
    public TextMeshProUGUI deckPriceText;
    public int deckPrice;
    public Button deckButton;

    public List<Card> _deck = new List<Card>();    
    public List<Card> _hand = new List<Card>();
    private DeckManager _deckManager => DeckManager.Instance;

    private void Awake()
    {
        deckButton.onClick.AddListener(OnDeckClicked);
        deckPrice = 10;
        deckPriceText.text = deckPrice.ToString("F0");
    }

    private void Start()
    {
        _deck = _deckManager.Shuffle();
        UpdateDeckPrice();
        DrawStartingHand();
    }

    private void UpdateDeckPrice()
    {
        deckPrice += 10;
        deckPriceText.text = deckPrice.ToString("F0");
    }

    void DrawStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
            DrawCardToHand();
    }

    public void DrawCardToHand()
    {
        if (_deck.Count == 0) return;

        Card cardData = _deck[_deck.Count - 1];
        _deck.RemoveAt(_deck.Count - 1);
        CardUI cardUI = Instantiate(cardUIPrefab, handPanel).GetComponent<CardUI>();
        cardUI.Initialize(cardData);
        _hand.Add(cardData);
        FindAnyObjectByType<HandLayout>()?.RepositionCards();
    }

    public void OnDeckClicked()
    {

        if (GameManager.Instance.coins >= deckPrice)
        {
            GameManager.Instance.UpdateCoins(-deckPrice);
            UpdateDeckPrice();
            DrawCardToHand();
        }
        else
        {
            Debug.Log("Not enough coins to draw a card.");
        }
    }
}
