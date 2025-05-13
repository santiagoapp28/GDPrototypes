using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("Deck Setup")]
    public Deck initialDeck;
    public int startingHandSize = 5;

    [Header("UI Setup")]
    public Transform handPanel;
    public Transform deckCornerPanel;
    public GameObject cardUIPrefab;

    public List<Card> deck = new List<Card>();
    private List<Card> hand = new List<Card>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeDeck();
        DrawStartingHand();
    }

    void InitializeDeck()
    {
        deck = new List<Card>(initialDeck.cards);
        Shuffle(deck);
    }

    void Shuffle(List<Card> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    void DrawStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
            DrawCardToHand();
    }

    public void DrawCardToHand()
    {
        if (deck.Count == 0) return;

        Card cardData = deck.Last();
        deck.Remove(deck.Last());
        Debug.Log(cardData.cardName);
        CardUI cardUI = Instantiate(cardUIPrefab, handPanel).GetComponent<CardUI>();
        cardUI.Initialize(cardData);
        hand.Add(cardData);
        FindAnyObjectByType<HandLayout>()?.RepositionCards();

        // Optionally update deckCorner UI too
    }

    public void OnDeckClicked() //clicked on new card draw
    {
        DrawCardToHand();
    }
}
