using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Tower Defense/GameConfig")]
public class GameConfig : ScriptableObject
{
    public int towerMaxHeight = 5;
    public int startingCoins = 100;
    public int startingHealth = 100;
    public int startingEnergy = 50;
    public int maxHandCount = 10;
    public List<int> energyCostsPerDraw = new List<int> { 100, 120, 150, 200 };
}
