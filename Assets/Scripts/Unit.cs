using UnityEngine;

public class Unit : MonoBehaviour {
    public UnitMoveType moveType;
    public Vector2Int pos;
    public int moveDistance;
    public bool moveDistanceReduced;
    public int moveDistanceReductionFactor;
    public int maxHealth;
    public int health;
    public bool hasFuel;
    public int maxFuel;
    public int fuel;

    public Strategia.Grid gridScript;
    public Tile[,] grid;

    private bool[] moveDirs = new bool[8];
    // Move direction go from left to right, top to bottom
    // E.G. Left and Up = 1, Up = 2, Right and Up = 3 etc...

    public void Start() {
        grid = GameObject.Find("Grid").GetComponent<Strategia.Grid>().grid;
    }

    public void CheckDirs(Tile[,] grid) {
        TileType[] tiles = new TileType[8];
        tiles[0] = grid[pos.x-1, pos.y-1].tileType;
        tiles[1] = grid[pos.x  , pos.y-1].tileType;
        tiles[2] = grid[pos.x+1, pos.y-1].tileType;
        tiles[3] = grid[pos.x-1, pos.y  ].tileType;
        tiles[4] = grid[pos.x+1, pos.y  ].tileType;
        tiles[5] = grid[pos.x-1, pos.y+1].tileType;
        tiles[6] = grid[pos.x  , pos.y+1].tileType;
        tiles[7] = grid[pos.x+1, pos.y+1].tileType;

        switch (moveType) {
            case UnitMoveType.Air:
                for (int i = 0; i < tiles.Length; i++) {
                    if (tiles[i] == TileType.Mountains) {
                        moveDirs[i] = false;
                    } else {
                        moveDirs[i] = true;
                    }
                }
                break;
            case UnitMoveType.Land:
                for (int i = 0; i < tiles.Length; i++) {
                    if ((tiles[i] == TileType.Sea) || (tiles[i] == TileType.Trees)) {
                        moveDirs[i] = false;
                    } else {
                        moveDirs[i] = true;
                    }
                }
                break;
            case UnitMoveType.Sea:
                for (int i = 0; i < tiles.Length; i++) {
                    if (tiles[i] != TileType.Sea) {
                        moveDirs[i] = false;
                    } else {
                        moveDirs[i] = true;
                    }
                }
                break;
            default:
                break;
        }
    }
}

public enum UnitMoveType {Land, Air, Sea};

