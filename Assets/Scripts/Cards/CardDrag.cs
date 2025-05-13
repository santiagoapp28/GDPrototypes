using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private GameObject ghost;
    private GridManager gridManager;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        gridManager = FindAnyObjectByType<GridManager>();
    }

    private void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        // Spawn ghost
        ghost = Instantiate(GetComponent<CardUI>().segmentPrefab);
        SetGhostVisuals(ghost, true); // make it transparent/uninteractive
    }

    Vector2Int gridPos;
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Update ghost position
        if (ghost && gridManager)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (gridManager != null)
                {
                    Vector3 worldPos2 = hit.point;
                    gridPos = new Vector2Int(
                        Mathf.FloorToInt((worldPos2.x - gridManager.transform.position.x) / gridManager.tileSize),
                        Mathf.FloorToInt((worldPos2.z - gridManager.transform.position.z) / gridManager.tileSize)
                    );
                    int height = gridManager.GetTowerHeight(gridPos);
                    Vector3 snappedPos = gridManager.GetSnappedWorldPosition(gridPos);
                    snappedPos.y += gridManager.tileSize; // Hover one tile above
                    ghost.transform.position = snappedPos;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var dragger = eventData.pointerDrag?.GetComponent<CardDrag>();
        if (dragger != null)
        {
            Vector3 ghostPos = dragger.GetGhostPosition(); // You’ll create this method
            Vector3 spawnPos = ghostPos + new Vector3(0, -gridManager.tileSize, 0);
            gridManager.PlaceTowerSegment(gridPos, dragger.GetComponent<CardUI>().segmentPrefab);

            dragger.DestroyGhost();
            Destroy(eventData.pointerDrag); // Remove card from hand
        }
    }

    public void DestroyGhost()
    {
        if (ghost)
        {
            Destroy(ghost);
        }
    }

    private void SetGhostVisuals(GameObject obj, bool isGhost)
    {
        // Disable or enable all colliders
        foreach (var col in obj.GetComponentsInChildren<Collider>())
            col.enabled = !isGhost;

        // Optional: Prevent ghost from being raycast target
        obj.layer = isGhost ? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");
    }

    public Vector3 GetGhostPosition()
    {
        return ghost ? ghost.transform.position : Vector3.zero;
    }


}
