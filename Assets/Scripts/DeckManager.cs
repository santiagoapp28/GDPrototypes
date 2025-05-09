using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("Deck Setup")]
    public List<Card> allPossibleCards;  // assign in Inspector
    public int startingHandSize = 5;

    [Header("UI Setup")]
    public Transform handPanel;
    public Transform deckCornerPanel;
    public GameObject cardUIPrefab;

    private Queue<Card> deck = new Queue<Card>();
    private List<GameObject> handCards = new List<GameObject>();

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
        List<Card> temp = new List<Card>(allPossibleCards);
        Shuffle(temp);

        foreach (var card in temp)
            deck.Enqueue(card);
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

        Card cardData = deck.Dequeue();
        GameObject cardUI = Instantiate(cardUIPrefab, handPanel);
        cardUI.GetComponent<CardUI>().Initialize(cardData);
        handCards.Add(cardUI);
        FindAnyObjectByType<HandLayout>()?.RepositionCards();

        // Optionally update deckCorner UI too
    }

    public void OnDeckClicked() //clicked on new card draw
    {
        DrawCardToHand();
    }
}
