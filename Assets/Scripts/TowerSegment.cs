using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSegment : MonoBehaviour, IDropHandler
{
    Tower _currentTower;
    public Vector2Int gridPosition;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Drop detected at {gridPosition}");

        var dragger = eventData.pointerDrag?.GetComponent<CardDrag>();
        if (dragger != null)
        {
            FindAnyObjectByType<GridManager>().TryPlaceTowerSegment(gridPosition, dragger.GetComponent<CardUI>().segmentPrefab);
            dragger.DestroyGhost();
            Destroy(eventData.pointerDrag); // Remove card from hand
        }
    }
}
