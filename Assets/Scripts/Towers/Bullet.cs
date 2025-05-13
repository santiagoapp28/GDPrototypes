using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float lifespan = 5f;

    private void Start()
    {
        Destroy(gameObject, lifespan); // Destroy after lifespan
    }
    
    public void InitBullet(float newSpeed, float newDamage)
    {
        speed = newSpeed;
        damage = newDamage;
    }

    private void Update()
    {
        Move();   
    }

    private void Move()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object can be damaged
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject); // Bullet disappears on hit
        }
        else if (!other.isTrigger) // Hit a wall or something solid
        {
            Destroy(gameObject);
        }
    }
}
