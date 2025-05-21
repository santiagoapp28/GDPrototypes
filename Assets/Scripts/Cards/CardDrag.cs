using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private GameObject ghost;
    private GridManager gridManager;

    private Vector3 _originalPosition;
    private Transform _originalParent;

    private bool _dragCancelled;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        gridManager = FindAnyObjectByType<GridManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            // Right-click to cancel drag
            if (ghost)
            {
                _dragCancelled = true;
                gridManager.StopTileHighlights(); // Stop tile highlight

                Destroy(ghost);
                ResetCardPosition();
                EnableCardInteractivity();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragCancelled = false; // Reset cancel flag when drag starts

        _originalPosition = transform.position;
        _originalParent = transform.parent;

        transform.SetParent(transform.root); // Detach to top level for dragging
        canvasGroup.blocksRaycasts = false;

        // Spawn ghost
        ghost = Instantiate(GetComponent<CardUI>().segmentPrefab);
        SetGhostVisuals(ghost, true); // make it transparent/uninteractive

        AudioManager.Instance.PlaySFX(Sounds.UIClick);
    }

    Vector2Int gridPos;
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragCancelled)
        {
            return; // Skip dragging logic
        }

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition = new Vector2(
            Mathf.Clamp(rectTransform.anchoredPosition.x, 
            -canvas.GetComponent<RectTransform>().rect.width/2, canvas.GetComponent<RectTransform>().rect.width),
            Mathf.Clamp(rectTransform.anchoredPosition.y,
            -canvas.GetComponent<RectTransform>().rect.height/2, -canvas.GetComponent<RectTransform>().rect.height / 4)
        );

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
                        Mathf.FloorToInt((worldPos2.x - gridManager.transform.position.x) / GridManager.tileSize),
                        Mathf.FloorToInt((worldPos2.z - gridManager.transform.position.z) / GridManager.tileSize)
                    );
                    gridPos = new Vector2Int(Mathf.Clamp(gridPos.x, 0, gridManager.width - 1), 
                        Mathf.Clamp(gridPos.y, 0, gridManager.height - 1));
                    int height = gridManager.GetTowerHeight(gridPos);
                    Vector3 snappedPos = gridManager.GetSnappedWorldPosition(gridPos);
                    gridManager.TileHighlight(gridPos); // Highlight tile
                    snappedPos.y += GridManager.tileSize; // Hover one tile above
                    ghost.transform.position = snappedPos;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragCancelled)
        {
            _dragCancelled = false; // Reset for next drag
            return; // Skip placement logic
        }

        var dragger = eventData.pointerDrag?.GetComponent<CardDrag>();
        if (dragger != null)
        {
            Vector3 ghostPos = dragger.GetGhostPosition();
            Vector3 spawnPos = ghostPos + new Vector3(0, -GridManager.tileSize, 0);
            CardUI cardUI = dragger.GetComponent<CardUI>();
            CardType cardType = cardUI.cardType;

            bool placedSegment = gridManager.PlaceTowerSegment(gridPos, cardUI.segmentPrefab, cardType);
            dragger.DestroyGhost();
            gridManager.StopTileHighlights();

            if (placedSegment)
            {
                Destroy(eventData.pointerDrag); // Success: remove the card
            }
            else
            {
                // ✳️ Restore the card's position and re-enable interaction
                dragger.ResetCardPosition();
                dragger.EnableCardInteractivity();
            }
        }
    }

    public void ResetCardPosition()
    {
        transform.SetParent(_originalParent);
        transform.position = _originalPosition;
    }

    public void EnableCardInteractivity()
    {
        canvasGroup.blocksRaycasts = true;
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
