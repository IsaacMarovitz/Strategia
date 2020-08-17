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
    public UIInfo UIInfo;

    public Strategia.Grid gridScript;
    public Tile[,] grid;

    public int movesLeft;
    public bool[] moveDirs = new bool[8];
    public bool turnComplete = false;

    private bool selected = false;

    // Move direction go from left to right, top to bottom
    // E.G. Left and Up = 1, Up = 2, Right and Up = 3 etc...

    public void Update() {
        transform.position = new Vector3(pos.x * gridScript.tileWidth, transform.position.y, pos.y * gridScript.tileHeight);
        CheckDirs();
        if (selected) {
            UIInfo.pos = pos;
            UIInfo.worldPos = transform.position;
            UIInfo.movesLeft = movesLeft;
            UIInfo.moveDirs = moveDirs;
            if (UIInfo.newMove) {
                UIInfo.newMove = false;
                Move(UIInfo.dir);
            }
        }
    }

    public void Start() {
        movesLeft = moveDistance;
    }

    public void Selected() {
        selected = true;
    }

    public void Deselected() {
        selected = false;
    }

    public void NewTurn() {
        turnComplete = false;
        movesLeft = moveDistance;
    }

    public void CheckDirs() {
        if (movesLeft <= 0) {
            turnComplete = true;
            moveDirs =  new bool[]{ false, false, false, false, false, false, false, false };
        } else {
            if (grid == null) {
                grid = gridScript.grid;
            }
            Tile[] tiles = new Tile[8];
            tiles[0] = grid[pos.x - 1, pos.y + 1];
            tiles[1] = grid[pos.x, pos.y + 1];
            tiles[2] = grid[pos.x + 1, pos.y + 1];
            tiles[3] = grid[pos.x - 1, pos.y];
            tiles[4] = grid[pos.x + 1, pos.y];
            tiles[5] = grid[pos.x - 1, pos.y - 1];
            tiles[6] = grid[pos.x, pos.y - 1];
            tiles[7] = grid[pos.x + 1, pos.y - 1];

            switch (moveType) {
                case UnitMoveType.Air:
                    for (int i = 0; i < tiles.Length; i++) {
                        if (tiles[i].tileType == TileType.Mountains) {
                            moveDirs[i] = false;
                        } else {
                            moveDirs[i] = true;
                        }
                    }
                    break;
                case UnitMoveType.Land:
                    for (int i = 0; i < tiles.Length; i++) {
                        if ((tiles[i].tileType == TileType.Sea) || (tiles[i].tileType == TileType.Trees)) {
                            moveDirs[i] = false;
                        } else {
                            moveDirs[i] = true;
                        }
                    }
                    break;
                case UnitMoveType.Sea:
                    for (int i = 0; i < tiles.Length; i++) {
                        if (tiles[i].tileType != TileType.Sea) {
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

    public void Move(int dir) {
        movesLeft--;
        switch (dir) {
            case 1:
                pos.x--;
                pos.y++;
                break;
            case 2:
                pos.y++;
                break;
            case 3:
                pos.x++;
                pos.y++;
                break;
            case 4:
                pos.x--;
                break;
            case 5:
                pos.x++;
                break;
            case 6:
                pos.x--;
                pos.y--;
                break;
            case 7:
                pos.y--;
                break;
            case 8:
                pos.x++;
                pos.y--;
                break;
            default:
                break;
        }
    }
}

public enum UnitMoveType { Land, Air, Sea };

