using UnityEngine;
using System.Collections.Generic;

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
    private List<Tile> oldTiles = new List<Tile>();

    // Move direction go from left to right, top to bottom
    // E.G. Left and Up = 1, Up = 2, Right and Up = 3 etc...

    public void Update() {
        transform.position = new Vector3(pos.x * gridScript.tileWidth, transform.position.y, pos.y * gridScript.tileHeight);
        if (selected) {
            UIInfo.pos = pos;
            UIInfo.unitWorldPos = transform.position;
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
        gridScript = GameObject.Find("Grid").GetComponent<Strategia.Grid>();
        CheckDirs();
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
            moveDirs = new bool[] { false, false, false, false, false, false, false, false };
        } else {
            if (grid == null) {
                grid = gridScript.grid;
            }
            Tile[] tiles = GridUtilities.DiagonalCheck(grid, gridScript.width, gridScript.height, pos);

            switch (moveType) {
                case UnitMoveType.Air:
                    for (int i = 0; i < tiles.Length; i++) {
                        if (tiles[i] == null) {
                            moveDirs[i] = false;
                        } else if (tiles[i].tileType == TileType.Mountains) {
                            moveDirs[i] = false;
                        } else {
                            moveDirs[i] = true;
                        }
                    }
                    break;
                case UnitMoveType.Land:
                    for (int i = 0; i < tiles.Length; i++) {
                        if (tiles[i] == null) {
                            moveDirs[i] = false;
                        } else if ((tiles[i].tileType == TileType.Sea) || (tiles[i].tileType == TileType.Trees)) {
                            moveDirs[i] = false;
                        } else {
                            moveDirs[i] = true;
                        }
                    }
                    break;
                case UnitMoveType.Sea:
                    for (int i = 0; i < tiles.Length; i++) {
                        if (tiles[i] == null) {
                            moveDirs[i] = false;
                        } else if (tiles[i].tileType != TileType.Sea) {
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
        if (movesLeft > 0) {
            movesLeft--;
            switch (dir) {
                case 1:
                    if (moveDirs[0]) {
                        pos.x--;
                        pos.y++;
                        this.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    break;
                case 2:
                    if (moveDirs[1]) {
                        pos.y++;
                        this.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    break;
                case 3:
                    if (moveDirs[2]) {
                        pos.x++;
                        pos.y++;
                        this.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    break;
                case 4:
                    if (moveDirs[3]) {
                        pos.x--;
                        this.transform.eulerAngles = new Vector3(0, -90, 0);
                    }
                    break;
                case 5:
                    if (moveDirs[4]) {
                        pos.x++;
                        this.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    break;
                case 6:
                    if (moveDirs[5]) {
                        pos.x--;
                        pos.y--;
                        this.transform.eulerAngles = new Vector3(0, -180, 0);
                    }
                    break;
                case 7:
                    if (moveDirs[6]) {
                        pos.y--;
                        this.transform.eulerAngles = new Vector3(0, -180, 0);
                    }
                    break;
                case 8:
                    if (moveDirs[7]) {
                        pos.x++;
                        pos.y--;
                        this.transform.eulerAngles = new Vector3(0, -180, 0);
                    }
                    break;
            }
            foreach (var tile in oldTiles) {
                tile.tileScript.ChangeVisibility(Visibility.Hidden);
            }
            List<Tile> nearbyTiles = GridUtilities.RadialSearch(gridScript.grid, pos, 5);
            foreach (var tile in nearbyTiles) {
                tile.tileScript.ChangeVisibility(Visibility.Visable);
            }
            oldTiles = nearbyTiles;
            CheckDirs();
        }
    }
}

public enum UnitMoveType { Land, Air, Sea };
