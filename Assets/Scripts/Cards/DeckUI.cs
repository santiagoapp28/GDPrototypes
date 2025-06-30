using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Deck Setup")]
    public int startingHandSize = 5;

    [Header("UI Setup")]
    public Transform handPanel;
    public Transform deckCornerPanel;
    public GameObject cardUIPrefab;
    public TextMeshProUGUI deckHoverText;
    public Button deckButton;

    public List<Card> _deck = new List<Card>();    
    public List<Card> _hand = new List<Card>();
    private DeckManager _deckManager => DeckManager.Instance;

    public int CurrentHandCount => _hand.Count;

    private void Awake()
    {
        deckButton.onClick.AddListener(OnDeckClicked);
    }

    private void Start()
    {
        _deck = _deckManager.Shuffle();
        DrawStartingHand();
        UpdateDeckCountText();
        if (deckHoverText != null)
            deckHoverText.gameObject.SetActive(false);

        // Programmatically add event triggers for hover to the deck button
        EventTrigger trigger = deckButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = deckButton.gameObject.AddComponent<EventTrigger>();

        // Pointer Enter
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(pointerEnterEntry);

        // Pointer Exit
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(pointerExitEntry);
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
        UpdateDeckCountText();
    }

    public void RemoveCardFromHand(Card card)
    {
        if (card != null)
        {
            _hand.Remove(card);
        }
    }

    public void OnDeckClicked()
    {
        var deckViewer = FindAnyObjectByType<DeckViewer>();
        if (deckViewer != null)
        {
            deckViewer.OpenDeckView();
        }
        else
        {
            Debug.LogWarning("DeckViewer not found in scene. Cannot open deck view.");
        }
    }

    public void DrawMultipleCards(int amountToDraw)
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            if (_deck.Count == 0)
            {
                Debug.Log("Deck is empty, cannot draw more cards.");
                break;
            }
            DrawCardToHand();
        }
    }

    private void UpdateDeckCountText()
    {
        if (deckHoverText != null)
            deckHoverText.text = $"{_deck.Count}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (deckHoverText != null)
            deckHoverText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (deckHoverText != null)
            deckHoverText.gameObject.SetActive(false);
    }
}
