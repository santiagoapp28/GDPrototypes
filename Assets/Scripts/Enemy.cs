using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public float speed = 5f;
    public int coins;
    public float detectionDistance = 3f;
    public float sideRayAngle = 30f; // degrees for side rays
    public float dodgeStrength = 0.5f; // 0 = full forward, 1 = strong sideways
    public LayerMask towerLayer;

    public static event Action<Enemy> OnEnemyDied;
    private WaveManager waveManager;

    private Vector3 originalDirection;
    private Vector3 dodgeDirection;
    private float dodgeTime = 0f;

    private Vector3 initialDirection;

    private void Start()
    {
        initialDirection = transform.forward.normalized; // Save the spawn direction
        waveManager = FindAnyObjectByType<WaveManager>();
    }

    void Update()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        Vector3 forward = transform.forward;
        Vector3 leftRay = Quaternion.Euler(0, -sideRayAngle, 0) * forward;
        Vector3 rightRay = Quaternion.Euler(0, sideRayAngle, 0) * forward;

        bool frontHit = Physics.Raycast(rayOrigin, forward, detectionDistance, towerLayer);
        bool leftHit = Physics.Raycast(rayOrigin, leftRay, detectionDistance, towerLayer);
        bool rightHit = Physics.Raycast(rayOrigin, rightRay, detectionDistance, towerLayer);

        Vector3 avoidance = Vector3.zero;
        if (frontHit)
            avoidance += transform.right;
        if (leftHit)
            avoidance += transform.right;
        if (rightHit)
            avoidance -= transform.right;

        Vector3 blendedDir = Vector3.Lerp(initialDirection, initialDirection + avoidance, dodgeStrength);
        Vector3 finalDir = blendedDir.normalized;
        transform.position += finalDir * speed * Time.deltaTime;

        Quaternion targetRot = Quaternion.LookRotation(finalDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{name} took {amount} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.Instance.AddCoins(10);
        // Notify subscribers (like WaveSpawner or GameManager)
        OnEnemyDied?.Invoke(this);

        // Optional: Give player gold/score via a GameManager
        // if (GameManager.Instance != null)
        // {
        //     GameManager.Instance.AddGold(value);
        // }

        // Add explosion effect, sound, etc.
        Destroy(gameObject);
    }

    void ReachedEndOfPath()
    {
        // Notify subscribers (WaveSpawner needs to know it wasn't "killed" but still gone)
        OnEnemyDied?.Invoke(this); // Treat as "dead" for wave counting purposes

        // Optional: Player loses a life via a GameManager
        // if (GameManager.Instance != null)
        // {
        //     GameManager.Instance.LoseLife();
        // }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector3 forward = transform.forward;
        Vector3 leftRay = Quaternion.Euler(0, -sideRayAngle, 0) * forward;
        Vector3 rightRay = Quaternion.Euler(0, sideRayAngle, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + forward * detectionDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftRay * detectionDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightRay * detectionDistance);
    }
}

public interface IDamageable
{
    void TakeDamage(float amount);
}