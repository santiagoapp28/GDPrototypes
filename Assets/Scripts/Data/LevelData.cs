using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Data
{
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Tower Defense/Level Data", order = 51)]
    public class LevelData : ScriptableObject
    {
        public int width;
        public int height;
        public List<TileData> grid = new List<TileData>();

        public void InitializeGrid()
        {
            grid = new List<TileData>();
            for (int i = 0; i < width * height; i++)
            {
                grid.Add(new TileData());
            }
        }

        public TileData GetTile(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return grid[y * width + x];
            }
            return null;
        }

        public void SetTile(int x, int y, TileData tile)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                grid[y * width + x] = tile;
            }
        }

        public void ResizeGrid(int newWidth, int newHeight)
        {
            List<TileData> newGrid = new List<TileData>();
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    if (x < width && y < height)
                    {
                        newGrid.Add(GetTile(x, y));
                    }
                    else
                    {
                        newGrid.Add(new TileData());
                    }
                }
            }

            grid = newGrid;
            width = newWidth;
            height = newHeight;
        }
    }
}