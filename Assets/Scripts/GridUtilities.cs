using UnityEngine;
using System.Collections.Generic;

public static class GridUtilities {
    // Check if surrounding tiles (not including diagonals) are sea tiles
    public static bool CostalCheck(Vector2Int pos) {
        int width = GameManager.Instance.grid.width;
        int height = GameManager.Instance.grid.height;
        Tile[,] grid = GameManager.Instance.grid.grid;
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

    // Return all surrounding tiles including diagonals
    public static Tile[] DiagonalCheck(Vector2Int pos) {
        Tile[,] grid = GameManager.Instance.grid.grid;
        int width = GameManager.Instance.grid.width;
        int height = GameManager.Instance.grid.height;
        Tile[] tiles = new Tile[8];
        if (pos.x > 0 && pos.y < height - 1) {
            tiles[0] = grid[pos.x - 1, pos.y + 1];
        }
        if (pos.y < height - 1) {
            tiles[1] = grid[pos.x, pos.y + 1];
        }
        if (pos.x < width - 1 && pos.y < height - 1) {
            tiles[2] = grid[pos.x + 1, pos.y + 1];
        }
        if (pos.x > 0) {
            tiles[3] = grid[pos.x - 1, pos.y];
        }
        if (pos.x < width - 1) {
            tiles[4] = grid[pos.x + 1, pos.y];
        }
        if (pos.x > 0 && pos.y > 0) {
            tiles[5] = grid[pos.x - 1, pos.y - 1];
        }
        if (pos.y > 0) {
            tiles[6] = grid[pos.x, pos.y - 1];
        }
        if (pos.x < width - 1 && pos.y > 0) {
            tiles[7] = grid[pos.x + 1, pos.y - 1];
        }
        return tiles;
    }

    // Perform a radial search of surrounding tiles (includes starting tile in return list)
    public static List<Tile> RadialSearch(Vector2Int pos, int radius) {
        Tile[,] grid = GameManager.Instance.grid.grid;
        List<Tile> tilesInRange = new List<Tile>();
        Tile startingPoint = grid[pos.x, pos.y];
        foreach (var tile in grid) {
            if ((Mathf.Pow((startingPoint.pos.x - tile.pos.x), 2) + (Mathf.Pow((startingPoint.pos.y - tile.pos.y), 2))) < Mathf.Pow(radius, 2)) {
                tilesInRange.Add(tile);
            }
        }
        return tilesInRange;
    }

    // Check for a tile 'island' using a flood fill algorithm
    public static List<Tile> FloodFill(Tile startingPoint) {
        Tile[,] grid = GameManager.Instance.grid.grid;
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

            nextTile = grid[n.pos.x - 1, n.pos.y];
            if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                islandTiles.Add(nextTile);
                tileQueue.Enqueue(nextTile);
            }
            nextTile = grid[n.pos.x + 1, n.pos.y];
            if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                islandTiles.Add(nextTile);
                tileQueue.Enqueue(nextTile);
            }
            nextTile = grid[n.pos.x, n.pos.y + 1];
            if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                islandTiles.Add(nextTile);
                tileQueue.Enqueue(nextTile);
            }
            nextTile = grid[n.pos.x - 1, n.pos.y - 1];
            if (nextTile.tileType != TileType.Sea && !islandTiles.Contains(nextTile)) {
                islandTiles.Add(nextTile);
                tileQueue.Enqueue(nextTile);
            }
        }
        return islandTiles;
    }

    // Find the shortest path between two tiles with A*
    public static void FindPath(Tile startTile, Tile targetTile, List<TileType> tileTypes) {
        Heap<Tile> openSet = new Heap<Tile>(GameManager.Instance.grid.MaxSize);
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while (openSet.Count > 0) {
            Tile currentTile = openSet.RemoveFirst();
            closedSet.Add(currentTile);

            if (currentTile == targetTile) {
                RetracePath(startTile, targetTile);
                return;
            }

            foreach (Tile neighbour in DiagonalCheck(currentTile.pos)) {
                if (neighbour == null) {
                    continue;
                }
                if (neighbour.unitOnTile != null) {
                    if (neighbour.tileType == TileType.City || neighbour.tileType == TileType.CostalCity) {
                        neighbour.walkable = true;
                    } else {
                        Debug.Log(neighbour.gameObject.name);
                        neighbour.walkable = false;
                    }
                } else if (tileTypes.Contains(neighbour.tileType)) {
                    neighbour.walkable = false;
                }
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentTile.gCost + GetDistance(currentTile, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetTile);
                    neighbour.parent = currentTile;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    static void RetracePath(Tile startTile, Tile endTile) {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile) {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Add(startTile);
        path.Reverse();
        GameManager.Instance.grid.path = path;
    }

    static int GetDistance(Tile tileA, Tile tileB) {
        int distX = Mathf.Abs(tileA.pos.x - tileB.pos.x);
        int distY = Mathf.Abs(tileA.pos.y - tileB.pos.y);

        if (distX > distY) {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}