using UnityEngine;
using System.Collections.Generic;

public static class GridUtilities {
    // Check if surrounding tiles (not including diagonals) are sea tiles
    public static bool CostalCheck(Tile[,] grid, int width, int height, Vector2Int pos) {
        if ((pos.x > 0) && (grid[pos.x - 1, pos.y].tileType == TileType.Sea)) {
            return true;
        } else if ((pos.y > 0) && (grid[pos.x, pos.y - 1].tileType == TileType.Sea)) {
            return true;
        } else if ((pos.x < width - 1) && (grid[pos.x + 1, pos.y].tileType == TileType.Sea)) {
            return true;
        } else if ((pos.y < height - 1) && (grid[pos.x, pos.y + 1].tileType == TileType.Sea)) {
            return true;
        } else {
            return false;
        }
    }

    // Perform a radial search of surrounding tiles (includes starting tile in return list)
    public static List<Tile> RadialSearch(Tile[,] grid, Vector2Int pos, int radius) {
        List<Tile> tilesInRange = new List<Tile>();
        Tile startingPoint = grid[pos.x, pos.y];
        foreach (var tile in grid) {
            if ((Mathf.Pow((startingPoint.index.x - tile.index.x), 2) + (Mathf.Pow((startingPoint.index.y - tile.index.y), 2))) < Mathf.Pow(radius, 2)) {
                tilesInRange.Add(tile);
            }
        }
        return tilesInRange;
    }

    // Check for a tile 'island' using a flood fill algorithm
    public static List<Tile> FloodFill(Tile[,] grid, Tile startingPoint) {
        if (startingPoint.tileType == TileType.Sea) {
            return null;
        }
        List<Tile> islandTiles = new List<Tile>();
        Queue<Tile> tileQueue = new Queue<Tile>();
        islandTiles.Add(startingPoint);
        tileQueue.Enqueue(startingPoint);

        while (tileQueue.Count > 0) {
            Tile n = tileQueue.Peek();
            tileQueue.Dequeue();
            Tile nextTile;

            nextTile = grid[n.index.x - 1, n.index.y];
            if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                islandTiles.Add(nextTile);
                tileQueue.Enqueue(nextTile);
            }
            nextTile = grid[n.index.x + 1, n.index.y];
            if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                islandTiles.Add(nextTile);
                tileQueue.Enqueue(nextTile);
            }
            nextTile = grid[n.index.x, n.index.y + 1];
            if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                islandTiles.Add(nextTile);
                tileQueue.Enqueue(nextTile);
            }
            nextTile = grid[n.index.x - 1, n.index.y - 1];
            if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                islandTiles.Add(nextTile);
                tileQueue.Enqueue(nextTile);
            }
        }
        return islandTiles;
    }
}