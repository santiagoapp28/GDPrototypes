using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Tower Defense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public int health;
    public int armor;
    public float speed;
    public int damage;
    public int coinsOnDeath;
    public int energyOnDeath;
    public float acidResistant;
    public float fireResistant;
    public float iceResistant;
}