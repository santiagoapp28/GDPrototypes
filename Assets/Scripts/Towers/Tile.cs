using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool canPlaceTower = true;
    public GameObject negativePlacementFeedback;
    public GameObject positivePlacementFeedback;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
            canPlaceTower = false;
        }
    }

    public void StartHighlight()
    {
        if (canPlaceTower)
        {
            positivePlacementFeedback.SetActive(true);
        }
        else
        {
            negativePlacementFeedback.SetActive(true);
        }
    }

    public void StopHighlight()
    {
        negativePlacementFeedback.SetActive(false);
        positivePlacementFeedback.SetActive(false);
    }
}
