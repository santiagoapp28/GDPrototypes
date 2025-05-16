using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("Deck Setup")]
    public Deck initialDeck;
    public List<Card> deck = new List<Card>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
        InitializeDeck();
    }

    void InitializeDeck()
    {
        deck = new List<Card>(initialDeck.cards);
        deck = Shuffle();
    }

    public List<Card> Shuffle()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
        return new List<Card>(deck);
    }

    public void AddCard(Card currentCard)
    {
        deck.Add(currentCard);
    }
}
