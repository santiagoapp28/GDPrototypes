using UnityEngine;

[System.Serializable] // Makes it visible in the Inspector when used in a List/Array
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
    [Tooltip("Time in seconds between spawning each enemy in this group.")]
    public float spawnInterval = 1f;
    [Tooltip("Time in seconds to wait before this group starts spawning after the previous group (or wave start).")]
    public float startDelay = 0f; // Delay before this specific group starts spawning
}