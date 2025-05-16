using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite icon;
    public string description;
    public Color color;
    public GameObject segmentPrefab;
    public CardType cardType;
    public int shopPrice;
}

public enum CardType
{
    Base_Cannon,
    Base_Missile,
    Modifier_FireRate,
    Modifier_Damage,
    Modifier_Ice,
    Utiliy_TBD
}
