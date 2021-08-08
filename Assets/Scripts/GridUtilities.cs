using UnityEngine;
using System.Collections.Generic;

public static class GridUtilities {
    // Check if surrounding tiles (not including diagonals) are sea tiles
    public static bool CostalCheck(Vector2Int pos) {
        int width = GameManager.Instance.tileGrid.width;
        int height = GameManager.Instance.tileGrid.height;
        Tile[,] grid = GameManager.Instance.tileGrid.grid;
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
        Tile[,] grid = GameManager.Instance.tileGrid.grid;
        int width = GameManager.Instance.tileGrid.width;
        int height = GameManager.Instance.tileGrid.height;
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
        Tile[,] grid = GameManager.Instance.tileGrid.grid;
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
        Tile[,] grid = GameManager.Instance.tileGrid.grid;
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
    public static List<Tile> FindPath(Tile startTile, Tile targetTile, out bool goesThroughHiddenTiles) {
        Heap<Tile> openSet = new Heap<Tile>(GameManager.Instance.tileGrid.MaxSize);
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);
        goesThroughHiddenTiles = false;

        while (openSet.Count > 0) {
            Tile currentTile = openSet.RemoveFirst();
            closedSet.Add(currentTile);

            if (currentTile == targetTile) {
                return RetracePath(startTile, targetTile);
            }

            foreach (Tile neighbour in DiagonalCheck(currentTile.pos)) {
                if (neighbour == null) {
                    continue;
                }

                TileMoveStatus tileMoveStatus = UIData.currentUnit.CheckDir(neighbour);
                neighbour.walkable = true;

                if (UIData.currentUnit.player.fogOfWarMatrix[neighbour.pos.x, neighbour.pos.y] == FogOfWarState.Hidden) {
                    goesThroughHiddenTiles = true;
                } else {
                    if (UIData.currentUnit.player.fogOfWarMatrix[neighbour.pos.x, neighbour.pos.y] == FogOfWarState.Revealed) {
                        goesThroughHiddenTiles = true;
                    }
                    if (tileMoveStatus == TileMoveStatus.Blocked) {
                        neighbour.walkable = false;
                    }
                    if (neighbour != targetTile && tileMoveStatus == TileMoveStatus.Attack) {
                        neighbour.walkable = false;
                    }
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

        return null;
    }

    // Calculate the world position of a given grid tile given the tile pos
    public static Vector3 TileToWorldPos(Vector2Int pos) {
        return new Vector3(pos.x * GameManager.Instance.tileGrid.tileWidth, 0, pos.y * GameManager.Instance.tileGrid.tileHeight);
    }

    public static Vector3 TileToWorldPos(Vector2Int pos, float y) {
        return new Vector3(pos.x * GameManager.Instance.tileGrid.tileWidth, y, pos.y * GameManager.Instance.tileGrid.tileHeight);
    }

    public static Vector3[] TilesToWorldPos(List<Tile> tiles) {
        Vector3[] positions = new Vector3[tiles.Count];
        for (int i = 0; i < tiles.Count; i++) {
            positions[i] = GridUtilities.TileToWorldPos(tiles[i].pos);
        }
        return positions;
    }

    static List<Tile> RetracePath(Tile startTile, Tile endTile) {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile) {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Add(startTile);
        path.Reverse();
        return path;
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