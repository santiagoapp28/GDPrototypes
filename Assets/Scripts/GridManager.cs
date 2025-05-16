using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width;
    public int height;
    public float tileSize = 1f;

    [Header("Tile Prefabs")]
    public GameObject tilePrefab; // Optional visual
    public GameObject tileDropColliderPrefab; // Optional drop area prefab

    private Dictionary<Vector2Int, int> towerHeights = new Dictionary<Vector2Int, int>();
    private TowerManager _towerMng;

    private void Start()
    {
        GenerateGrid();
        _towerMng = FindAnyObjectByType<TowerManager>();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector2Int gridPos = new Vector2Int(x, z);
                Vector3 worldPos = transform.position + new Vector3(x * tileSize, 0, z * tileSize);

                GameObject tile = new GameObject($"Tile_{x}_{z}");
                tile.transform.SetParent(transform);
                tile.transform.position = worldPos;
                tile.layer = LayerMask.NameToLayer("Tile");

                // Collider for drop detection
                BoxCollider collider = tile.AddComponent<BoxCollider>();
                collider.size = new Vector3(tileSize, 0.2f, tileSize);

                // Optional visual
                if (tilePrefab != null)
                {
                    Instantiate(tilePrefab, worldPos, Quaternion.identity, tile.transform);
                }
            }
        }
    }

    public Vector3 GetSnappedWorldPosition(Vector2Int gridPos)
    {
        int height = GetTowerHeight(gridPos);
        Vector3 offset = new Vector3(gridPos.x * tileSize, (height * tileSize)-1, gridPos.y * tileSize);
        return transform.position + offset;
    }

    public void PlaceTowerSegment(Vector2Int gridPos, GameObject towerSegmentPrefab, CardType cardtype)
    {
        Vector3 worldPos = GetSnappedWorldPosition(gridPos);
        GameObject towerSegment = Instantiate(towerSegmentPrefab, worldPos, Quaternion.identity, transform);

        TowerSegment newTowerSegment = towerSegment.GetComponent<TowerSegment>();
        newTowerSegment.gridPosition = gridPos;
        newTowerSegment.cardtype = cardtype;

        if (_towerMng.towerList.TryGetValue(gridPos, out Tower tower))
        {
            tower.AddSegment(newTowerSegment);
        }
        else
        {
            _towerMng.AddTower(gridPos, worldPos, newTowerSegment);
        }
        towerHeights[gridPos] = GetTowerHeight(gridPos) + 1;
    }

    public int GetTowerHeight(Vector2Int gridPos)
    {
        return towerHeights.TryGetValue(gridPos, out int height) ? height : 0;
    }
}
