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
        // Set the selected deck for the new run
        DeckManager.Instance.deck = deck.cards;

        // Reset game progress to start a fresh run on Easy difficulty
        GameManager.Instance.currentStageIndex = -1;
        GameManager.Instance.GetConfigValues();

        StageManager stageManager = GameManager.Instance.GetComponent<StageManager>();
        stageManager.StartNewStage();
    }

    private void OnViewDeck()
    {
        _deckViewer.OpenDeckView(deck);
    }
}
