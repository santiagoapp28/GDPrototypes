using UnityEngine;

[CreateAssetMenu(fileName = "Relic", menuName = "Tower Defense/Relic")]
public class Relic : ScriptableObject
{
    [field: SerializeField] public string relicName { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public Sprite image { get; private set; }
    [field: SerializeField] public int shopPrice { get; private set; }
    [field: SerializeField] public Rarity rarity { get; private set; }
    [field: SerializeField] public RelicType relicType { get; private set; }
}

public enum RelicType
{
    Handsize,
    ExtraCoins,
    HandsizePlus,
    ExtraCoinsPlus,
}

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
