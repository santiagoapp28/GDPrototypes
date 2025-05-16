using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Deck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    public string deckName;
    public string deckDescription;
    public Color deckColor;
    public List<Card> cards = new List<Card>();
}
