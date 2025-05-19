using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Tower Defense/GameConfig")]
public class GameConfig : ScriptableObject
{
    public int towerMaxHeight = 5;
    public int startingCoins = 100;
}
