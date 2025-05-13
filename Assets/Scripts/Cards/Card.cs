using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite icon;
    public string description;
    public Color color;
    public GameObject segmentPrefab;
    // Add any stats like damage, cooldown, etc.
}
