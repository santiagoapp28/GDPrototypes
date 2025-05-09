using UnityEngine;
using UnityEngine.EventSystems;

public class GridDropTarget : MonoBehaviour, IDropHandler
{
    public Vector2Int gridPosition;
    public GridManager gridManager;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Drop detected at {gridPosition}");

        var dragger = eventData.pointerDrag?.GetComponent<CardDrag>();
        if (dragger != null)
        {
            gridManager.TryPlaceTowerSegment(gridPosition, dragger.GetComponent<CardUI>().segmentPrefab);
            dragger.DestroyGhost();
            Destroy(eventData.pointerDrag); // Remove card from hand
        }
    }
}