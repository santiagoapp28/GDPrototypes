using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
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


        //PLACEHOLDER
        if(segment.type == 0)
        {
            canShoot = true;
        }
        else if (segment.type == 1)
        {
            bulletDamage *= 2;
        }
        else if (segment.type == 2)
        {
            fireRate *= 2;
        }
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
            Vector3 dir = (nearest.position - cannon.position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);
            Instantiate(bulletPrefab, cannon.position, rot);
            bulletPrefab.GetComponent<Bullet>().InitBullet(bulletSpeed, bulletDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
