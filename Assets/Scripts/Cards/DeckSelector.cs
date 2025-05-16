using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelector : MonoBehaviour
{
    public List<Deck> decks = new List<Deck>();
    public GameObject deckPrefab;

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject deckObject = Instantiate(deckPrefab, transform);
            Deck deck = decks[i];
            DeckSelectorItem deckSelectorItem = deckObject.GetComponent<DeckSelectorItem>();
            deckSelectorItem.Initialize(deck);
        }
    }
}
