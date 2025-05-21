using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width;
    public int height;
    public static float tileSize = 1f;

    [Header("Tile Prefabs")]
    public GameObject tilePrefab; // Optional visual
    public GameObject tileDropColliderPrefab; // Optional drop area prefab

    private Dictionary<Vector2Int, int> towerHeights = new Dictionary<Vector2Int, int>();
    private TowerManager _towerMng;

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

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

                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                tile.name = "Tile_" + x + "_" + z;
                tiles.Add(gridPos, tile.GetComponent<Tile>());
            }
        }
    }

    public Vector3 GetSnappedWorldPosition(Vector2Int gridPos)
    {
        int height = GetTowerHeight(gridPos);
        Vector3 offset = new Vector3(gridPos.x * tileSize, (height * tileSize)-1, gridPos.y * tileSize);
        return transform.position + offset;
    }

    Tile previousHighlight;
    public void TileHighlight(Vector2Int tile)
    {
        if (previousHighlight != null && previousHighlight != tiles[tile])
        {
            previousHighlight.StopHighlight();
        }
        previousHighlight = tiles[tile];
        if (tiles.TryGetValue(tile, out Tile tileComponent))
        {
            tileComponent.StartHighlight();
        }
    }

    public void StopTileHighlights()
    {
        if (previousHighlight != null)
        {
            previousHighlight.StopHighlight();
        }
    }

    public bool PlaceTowerSegment(Vector2Int gridPos, GameObject towerSegmentPrefab, CardType cardtype)
    {
        Vector3 worldPos = GetSnappedWorldPosition(gridPos);

        if (!tiles[gridPos].canPlaceTower)
        {
            Debug.Log("Tile is blocked by obstacle");
            return false;
        }

        if (towerHeights.TryGetValue(gridPos, out int height))
        {
            if(height >= GameManager.Instance.gameConfig.towerMaxHeight)
            {
                Debug.Log("Tower height limit reached");
                return false;
            }
        }

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

        return true;
    }

    public int GetTowerHeight(Vector2Int gridPos)
    {
        return towerHeights.TryGetValue(gridPos, out int height) ? height : 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GetSnappedWorldPosition(new Vector2Int(0, 0)), 0.2f);
        Gizmos.DrawSphere(GetSnappedWorldPosition(new Vector2Int(width - 1, height - 1)), 0.2f);
        Gizmos.DrawSphere(GetSnappedWorldPosition(new Vector2Int(0, height - 1)), 0.2f);
        Gizmos.DrawSphere(GetSnappedWorldPosition(new Vector2Int(width - 1, 0)), 0.2f);
    }
}
