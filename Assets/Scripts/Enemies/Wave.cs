using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWave", menuName = "Tower Defense/Wave")]
public class Wave : ScriptableObject
{
    [Tooltip("Optional name for this wave (for editor organization).")]
    public string waveName;
    public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    [Tooltip("Time in seconds before the next wave automatically starts after this one is cleared.")]
    public float timeToNextWave = 15f;
}