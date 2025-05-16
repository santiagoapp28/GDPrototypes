using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private TowerManager _towerMng;
    public List<TowerSegment> segments = new List<TowerSegment>();
    public Transform cannon;
    public GameObject bulletPrefab;
    public float detectionRadius = 20f;
    public LayerMask enemyLayer;
    public float bulletSpeed = 10f;
    public float bulletDamage = 10f;
    public float fireRate = 1f; // bullets per second (e.g. 1 shot per second)
    private float fireCooldown = 0f;

    public bool canShoot = false;

    private void Awake()
    {
        _towerMng = FindAnyObjectByType<TowerManager>();
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    public void AddSegment(TowerSegment segment)
    {
        segments.Add(segment);
        segment.transform.parent = transform;

        switch (segment.cardtype)
        {
            case CardType.Base_Cannon:
                canShoot = true;
                break;
            case CardType.Base_Missile:
                canShoot = true;
                bulletPrefab = _towerMng.ChangeBulletType(CardType.Base_Missile);
                break;
            case CardType.Modifier_Damage:
                bulletDamage *= 2;
                break;
            case CardType.Modifier_FireRate:
                fireRate *= 2;
                break;
            case CardType.Modifier_Ice:
                bulletPrefab = _towerMng.ChangeBulletType(CardType.Modifier_Ice);
                break;
            default:
                break;
        }

        AudioManager.Instance.PlaySFX(Sounds.PlaceSegment);
    }

    public void Shoot()
    {
        if (!canShoot) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }
            

        if (nearest != null)
        {
            Enemy enemy = nearest.GetComponentInParent<Enemy>();
            if (enemy == null) return;

            Vector3 targetPos = enemy.transform.position;
            Vector3 enemyForward = enemy.transform.forward;
            float enemySpeed = enemy.speed * (1f - enemy.slowDownEffect);

            // Calculate predicted point
            Vector3 predictedPos = FirstOrderIntercept(
                shooterPosition: cannon.position,
                shooterVelocity: Vector3.zero,
                shotSpeed: bulletSpeed,
                targetPosition: targetPos,
                targetVelocity: enemyForward * enemySpeed
            );

            Vector3 shootDir = (predictedPos - cannon.position).normalized;
            Quaternion rot = Quaternion.LookRotation(shootDir);
            Bullet newBullet = Instantiate(bulletPrefab, cannon.position, rot).GetComponent<Bullet>();
            newBullet.InitBullet(bulletSpeed, bulletDamage);


            ApplyEffects(newBullet);

            AudioManager.Instance.PlaySFX(Sounds.FireBullet);
        }
    }

    private void ApplyEffects(Bullet newBullet)
    {
        //If there is an ice segment, apply the slow effect
        float slowDownEffect = 0f;
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].cardtype == CardType.Modifier_Ice)
                slowDownEffect += 0.1f;
        }
        if (slowDownEffect > 0)
        {
            newBullet.GetComponent<Bullet>().effectType = EffectType.Slow;
            newBullet.GetComponent<Bullet>().effectAmount = slowDownEffect;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public static Vector3 FirstOrderIntercept(
    Vector3 shooterPosition,
    Vector3 shooterVelocity,
    float shotSpeed,
    Vector3 targetPosition,
    Vector3 targetVelocity)
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime(
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t * targetRelativeVelocity;
    }

    public static float FirstOrderInterceptTime(
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity)
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        // If a == 0 then target moves at bullet speed
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = 0.5f * targetRelativePosition.magnitude / shotSpeed;
            return t;
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        {
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
            float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            return Mathf.Min(t1, t2) > 0 ? Mathf.Min(t1, t2) : Mathf.Max(t1, t2);
        }
        else if (determinant < 0f)
        {
            return 0f; // No solution, aim directly
        }
        else
        {
            return -b / (2f * a);
        }
    }

}
