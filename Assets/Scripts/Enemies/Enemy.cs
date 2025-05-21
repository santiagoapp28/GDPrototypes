using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    private float _maxHealth;
    public float speed = 5f;
    public float slowDownEffect = 0f; //1f = 100% slow, 0f = no slow
    public int coins;
    public int damage = 10;
    public float detectionDistance = 3f;
    public float sideRayAngle = 30f; // degrees for side rays
    public float dodgeStrength = 0.5f; // 0 = full forward, 1 = strong sideways
    public LayerMask towerLayer;

    public static event Action<Enemy> OnEnemyDied;
    private WaveManager waveManager;
    private EnemyUI _enemyUI;
    private EnemyFeedback _enemyFeedback;

    private Vector3 initialDirection;

    private void Start()
    {
        initialDirection = transform.forward.normalized; // Save the spawn direction
        waveManager = FindAnyObjectByType<WaveManager>();
        _maxHealth = health;
        _enemyUI = GetComponent<EnemyUI>();
        _enemyFeedback = GetComponent<EnemyFeedback>();
        _enemyUI.UpdateHealthBar(health / _maxHealth);
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
        transform.position += finalDir * speed * (1 - slowDownEffect) * Time.deltaTime;

        Quaternion targetRot = Quaternion.LookRotation(finalDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        //Debug.Log($"{name} took {amount} damage. Remaining health: {health}");
        _enemyUI.UpdateHealthBar(health / _maxHealth);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            //AudioManager.Instance.PlaySFX(Sounds.BulletHit);
        }
    }

    void Die()
    {
        AudioManager.Instance.PlaySFX(Sounds.EnemyDeath);
        GameManager.Instance.UpdateCoins(coins);
        OnEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("House"))
        {
            ReachedEndOfPath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("House"))
        {
            ReachedEndOfPath();
        }
    }

    public void ReachedEndOfPath()
    {
        // Notify subscribers (WaveSpawner needs to know it wasn't "killed" but still gone)
        OnEnemyDied?.Invoke(this); // Treat as "dead" for wave counting purposes

        GameManager.Instance.UpdateHealth(-damage); // Reduce player health
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

    public void ReceiveEffect(EffectType effect, float amount)
    {
        switch (effect)
        {
            case EffectType.Slow:
                StartCoroutine(SlowEffect(amount));
                break;
            case EffectType.Stun:
                // Stop movement
                break;
            case EffectType.Poison:
                // Apply poison effect (e.g., reduce health over time)
                break;
            case EffectType.Burn:
                // Apply burn effect (e.g., reduce health over time)
                break;
            case EffectType.Freeze:
                // Stop movement
                break;
            default:
                break;
        }
    }

    public IEnumerator SlowEffect(float amount)
    {
        slowDownEffect += amount;
        _enemyFeedback.SlowDownFeedback(true); 
        while(slowDownEffect > 0)
        {
            yield return new WaitForSeconds(2f); // Duration of the slow effect
            slowDownEffect -= 0.1f; // Reset slow effect
            slowDownEffect = Mathf.Clamp(slowDownEffect, 0, 0.8f); // Ensure it doesn't go below 0
        }
        slowDownEffect = Mathf.Clamp(slowDownEffect, 0, 0.8f);
        if (slowDownEffect <= 0)
        {
            _enemyFeedback.SlowDownFeedback(false);
        }
    }
}

public interface IDamageable
{
    void TakeDamage(float amount);
    void ReceiveEffect(EffectType effect, float amount);
}

public enum EffectType
{
    None,
    Slow,
    Stun,
    Poison,
    Burn,
    Freeze
}