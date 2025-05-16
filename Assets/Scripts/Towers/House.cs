using UnityEngine;

public class House : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.ReachedEndOfPath();
            //reduce health
        }
    }
}
