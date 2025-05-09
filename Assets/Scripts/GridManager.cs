using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width;
    public int height;
    public float tileSize = 1f;

    public GameObject tilePrefab; // Optional visual prefab
    public GameObject tileDropColliderPrefab; // Invisible box for drop detection

    public Vector2Int selectedGridTile;

    public Transform tileParent;

    private Dictionary<Vector2Int, int> towerHeights = new Dictionary<Vector2Int, int>(); // Track the height for each tile


    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 localPos = new Vector3(x * tileSize, 0, z * tileSize);
                Vector3 worldPos = transform.position + localPos;

                GameObject tile = new GameObject($"Tile_{x}_{z}");
                tile.transform.parent = transform;
                tile.transform.position = worldPos;

                BoxCollider collider = tile.AddComponent<BoxCollider>();
                collider.size = new Vector3(tileSize, 0.2f, tileSize);

                tile.layer = LayerMask.NameToLayer("Tile");

                var dropTarget = tile.AddComponent<GridDropTarget>();
                dropTarget.gridManager = this;
                dropTarget.gridPosition = new Vector2Int(x, z);

                // Optional visual tile
                if (tilePrefab != null)
                {
                    Instantiate(tilePrefab, worldPos, Quaternion.identity, tile.transform);
                }
            }
        }
    }

    public Vector3 GetSnappedWorldPosition(Vector2Int gridPos)
    {
        height = GetTowerHeight(gridPos);
        Vector3 offset = new Vector3(gridPos.x * tileSize, (height * tileSize)-1f, gridPos.y * tileSize);
        return transform.position + offset;
    }

    public void TryPlaceTowerSegment(Vector2Int gridPos, GameObject towerSegmentPrefab)
    {
        Vector3 worldPos = GetSnappedWorldPosition(gridPos);
        GameObject towerSegment = Instantiate(towerSegmentPrefab, worldPos, Quaternion.identity);
        towerSegment.GetComponent<TowerSegment>().gridPosition = gridPos;

        towerHeights[gridPos] = GetTowerHeight(gridPos) + 1;
    }

    public int GetTowerHeight(Vector2Int gridPos)
    {
        // Return the current height (stacked towers) for the given grid position
        if (towerHeights.ContainsKey(gridPos))
        {
            return towerHeights[gridPos];
        }
        return 0;
    }
}
