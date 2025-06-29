using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Data;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public LevelData levelData;
    public static float tileSize = 1f;

    [Header("Tile Prefabs")]
    public GameObject tilePrefab; // Optional visual
    public GameObject obstaclePrefab;
    public GameObject castleTilePrefab;
    public GameObject enemyTilePrefab;

    public int GridWidth => levelData != null ? levelData.width : 0;
    public int GridHeight { get; private set; }
    private const int extraRowsPerEnd = 6;

    private Dictionary<Vector2Int, int> towerHeights = new Dictionary<Vector2Int, int>();
    private TowerManager _towerMng;

    private Transform _castleTileGroup;
    private Transform _enemyTileGroup;
    private Transform _obstacleGroup;
    private Transform _normalTileGroup;

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    private void Awake()
    {
        // Get LevelData from StageManager to allow for dynamic level loading
        StageManager stageManager = FindAnyObjectByType<StageManager>();
        if (stageManager != null)
        {
            levelData = stageManager.GetCurrentLevelData();
            if (levelData == null)
            {
                Debug.LogError("Failed to get LevelData from StageManager. Is the level list for the current difficulty populated?", this);
                enabled = false; // Disable to prevent errors
                return;
            }
        }
        else
        {
            Debug.LogWarning("StageManager not found. GridManager will use the LevelData assigned in the Inspector. This is intended for testing this scene directly.", this);
        }
    }

    private void Start()
    {
        GenerateGrid();
        _towerMng = FindAnyObjectByType<TowerManager>();
    }

    private void GenerateGrid()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned in GridManager.");
            return;
        }

        // Create parent GameObjects for organization
        _castleTileGroup = new GameObject("Castle Tiles").transform;
        _castleTileGroup.SetParent(transform);

        _enemyTileGroup = new GameObject("Enemy Tiles").transform;
        _enemyTileGroup.SetParent(transform);

        _obstacleGroup = new GameObject("Obstacles").transform;
        _obstacleGroup.SetParent(transform);

        _normalTileGroup = new GameObject("Normal Tiles").transform;
        _normalTileGroup.SetParent(transform);

        GridHeight = levelData.height + (extraRowsPerEnd * 2);

        // Calculate offset to center the grid around the GridManager's position
        float offsetX = (GridWidth - 1) * tileSize / 2.0f;
        float offsetZ = (GridHeight - 1) * tileSize / 2.0f;

        for (int x = 0; x < GridWidth; x++)
        {
            for (int z = 0; z < GridHeight; z++)
            {
                Vector2Int gridPos = new Vector2Int(x, z);
                // Apply offset to center the grid, and use the GridManager's height
                Vector3 worldPos = transform.position + new Vector3(x * tileSize - offsetX, 0, z * tileSize - offsetZ);

                GameObject prefabToInstantiate;
                Transform parentGroup;
                string tileTypeName;

                if (z < extraRowsPerEnd) // Castle area at the start of the grid (low z)
                {
                    prefabToInstantiate = castleTilePrefab;
                    parentGroup = _castleTileGroup;
                    tileTypeName = "CastleTile";
                }
                else if (z >= GridHeight - extraRowsPerEnd) // Enemy area at the end of the grid (high z)
                {
                    prefabToInstantiate = enemyTilePrefab;
                    parentGroup = _enemyTileGroup;
                    tileTypeName = "EnemyTile";
                }
                else // Middle section with regular tiles and obstacles
                {
                    int levelDataZ = z - extraRowsPerEnd;
                    TileData tileData = levelData.GetTile(x, levelDataZ);
                    if (tileData != null && tileData.isObstacle)
                    {
                        prefabToInstantiate = obstaclePrefab;
                        parentGroup = _obstacleGroup;
                        tileTypeName = "Obstacle";
                    }
                    else
                    {
                        prefabToInstantiate = tilePrefab;
                        parentGroup = _normalTileGroup;
                        tileTypeName = "Tile";
                    }
                }

                GameObject tileObject = Instantiate(prefabToInstantiate, worldPos, Quaternion.identity, parentGroup);
                tileObject.name = $"{tileTypeName}: X = {x}, Y = {z}";

                Tile tileComponent = tileObject.GetComponent<Tile>();
                if (tileComponent != null)
                {
                    tiles.Add(gridPos, tileComponent);
                }
            }
        }
    }

    public Vector3 GetSnappedWorldPosition(Vector2Int gridPos)
    {
        int height = GetTowerHeight(gridPos);

        // Calculate offset to center the grid around the GridManager's position
        float offsetX = (GridWidth - 1) * tileSize / 2.0f;
        float offsetZ = (GridHeight - 1) * tileSize / 2.0f;

        Vector3 offset = new Vector3(gridPos.x * tileSize - offsetX, height * tileSize, gridPos.y * tileSize - offsetZ);
        return transform.position + offset;
    }

    Tile previousHighlight;
    public void TileHighlight(Vector2Int tile)
    {
        // Try to get the tile component for the current grid position. This is safe even if the key doesn't exist.
        tiles.TryGetValue(tile, out Tile currentTileComponent);

        // If we were highlighting a tile previously, and it's not the same as the current one, stop highlighting it.
        if (previousHighlight != null && previousHighlight != currentTileComponent)
        {
            previousHighlight.StopHighlight();
        }

        // If we are over a tile (either placeable or an obstacle), highlight it.
        if (currentTileComponent != null)
        {
            currentTileComponent.StartHighlight();
        }

        // Remember the current tile for the next frame.
        previousHighlight = currentTileComponent;
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
        // Add one tile to the height to start towers off the ground.
        worldPos.y += tileSize;

        if (!tiles.TryGetValue(gridPos, out Tile tile) || !tile.canPlaceTower)
        {
            Debug.Log("Cannot place tower here. Tile is blocked or does not exist.");
            return false;
        }

        if (towerHeights.TryGetValue(gridPos, out int height))
        {
            if (GameManager.Instance == null || GameManager.Instance.gameConfig == null)
            {
                Debug.LogError("GameManager or GameConfig is not set. Cannot check tower height limit.");
                return false;
            }

            if (height >= GameManager.Instance.gameConfig.towerMaxHeight)
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
        if (levelData == null) return;

        Gizmos.DrawSphere(GetSnappedWorldPosition(new Vector2Int(0, 0)), 0.2f); // bottom-left
        Gizmos.DrawSphere(GetSnappedWorldPosition(new Vector2Int(GridWidth - 1, GridHeight - 1)), 0.2f); // top-right
        Gizmos.DrawSphere(GetSnappedWorldPosition(new Vector2Int(0, GridHeight - 1)), 0.2f); // top-left
        Gizmos.DrawSphere(GetSnappedWorldPosition(new Vector2Int(GridWidth - 1, 0)), 0.2f); // bottom-right
    }
}
