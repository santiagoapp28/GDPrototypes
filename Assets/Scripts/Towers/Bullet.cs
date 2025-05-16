using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float lifespan = 5f;
    public EffectType effectType;
    public float effectAmount = 0.1f;

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
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            if(effectType != EffectType.None)
            {
                damageable.ReceiveEffect(effectType, effectAmount);
            }
            Destroy(gameObject); // Bullet disappears on hit
        }
        else if (!other.isTrigger) // Hit a wall or something solid
        {
            Destroy(gameObject);
        }
    }
}
