using UnityEngine;

public class EnemyFeedback : MonoBehaviour
{
    public MeshRenderer enemyMesh;

    public Material normalMaterial;
    public Material iceMaterial;

    public void SlowDownFeedback(bool active)
    {
        if(active)
            enemyMesh.material = iceMaterial;
        else
            enemyMesh.material = normalMaterial;
    }
}
