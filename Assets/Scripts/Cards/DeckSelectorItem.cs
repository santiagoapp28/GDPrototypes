using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectorItem : MonoBehaviour
{
    public Deck deck;
    public string deckName;
    public TextMeshProUGUI deckNameText;
    public Button viewDeckButton;
    public Button selectDeckButton;
    public Image deckImage;
    public Color deckColor;
    private DeckViewer _deckViewer;

    public void Initialize(Deck newDeck)
    {
        deck = newDeck;
        deckName = deck.deckName;
        deckNameText.text = deckName;
        deckColor = deck.deckColor;
        viewDeckButton.onClick.AddListener(OnViewDeck);
        selectDeckButton.onClick.AddListener(OnSelectDeck);
        selectDeckButton.image.color = deckColor;
        deckImage.color = deckColor;

        _deckViewer = FindAnyObjectByType<DeckViewer>();
    }

    private void OnSelectDeck()
    {
        DeckManager.Instance.deck = deck.cards;
        GameManager.Instance.GetComponent<StageManager>().StartNewStage();
    }

    private void OnViewDeck()
    {
        _deckViewer.OpenDeckView(deck);
    }
}
