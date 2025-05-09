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

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        // Spawn ghost
        ghost = Instantiate(GetComponent<CardUI>().segmentPrefab);
        SetGhostVisuals(ghost, true); // make it transparent/uninteractive
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Update ghost position
        if (ghost && gridManager)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var grid = FindAnyObjectByType<GridManager>();
                if (grid != null)
                {
                    Vector3 worldPos2 = hit.point;
                    Vector2Int gridPos = new Vector2Int(
                        Mathf.FloorToInt((worldPos2.x - grid.transform.position.x) / grid.tileSize),
                        Mathf.FloorToInt((worldPos2.z - grid.transform.position.z) / grid.tileSize)
                    );

                    int height = grid.GetTowerHeight(gridPos);
                    Vector3 snappedPos = grid.GetSnappedWorldPosition(gridPos);

                    snappedPos = new Vector3(snappedPos.x, snappedPos.y + 1, snappedPos.z);
                    ghost.transform.position = snappedPos;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        DestroyGhost();
    }

    public void DestroyGhost()
    {
        if (ghost)
        {
            Destroy(ghost);
            Debug.Log("ghost destroyed");
        }
    }

    private void SetGhostVisuals(GameObject obj, bool isGhost)
    {
        foreach (var col in obj.GetComponentsInChildren<Collider>())
            col.enabled = !isGhost;

        foreach (var rend in obj.GetComponentsInChildren<Renderer>())
            rend.material.color = isGhost ? new Color(1, 1, 1, 0.5f) : Color.white;
    }

}
