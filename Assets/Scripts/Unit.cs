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

    public int movesLeft;
    public bool[] moveDirs = new bool[8];

    public bool isSleeping = false;
    public bool turnStarted = false;
    public bool turnComplete = false;
    
    private GameObject meshObject;
    private bool selected = false;
    private List<Tile> oldTiles = new List<Tile>();
    private Player player;

    // Move direction go from left to right, top to bottom
    // E.G. Left and Up = 1, Up = 2, Right and Up = 3 etc...

    public void NewDay(Player _player) {
        player = _player;
        movesLeft = moveDistance;
        turnStarted = false;
        turnComplete = false;
    }

    public void StartTurn() {
        turnStarted = true;
        turnComplete = false;
        movesLeft = moveDistance;
        CheckDirs();
        if (isSleeping) {
            EndTurn();
            return;
        }
        Selected();
    }

    public void EndTurn() {
        player.NextUnit(this, false);
        turnComplete = true;
    }

    public void Selected() {
        selected = true;
    }

    public void Deselected() {
        selected = false;
    }

    public void Update() {
        transform.position = new Vector3(pos.x * gridScript.tileWidth, transform.position.y, pos.y * gridScript.tileHeight);
        if (selected) {
            if (UIInfo.newMove) {
                UIInfo.newMove = false;
                Move(UIInfo.dir);
            }
        }
    }

    public void Start() {
        movesLeft = moveDistance;
        meshObject = gameObject.transform.GetChild(0).gameObject;
        CheckDirs();
        SetTileUnit();
    }

    public void CheckDirs() {
        if (movesLeft <= 0) {
            turnComplete = true;
            moveDirs = new bool[] { false, false, false, false, false, false, false, false };
        } else {
            Tile[] tiles = GridUtilities.DiagonalCheck(gridScript.grid, gridScript.width, gridScript.height, pos);

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
        gridScript.grid[pos.x, pos.y].tileScript.unitOnTile = null;
        movesLeft--;
        switch (dir) {
            case 1:
                if (moveDirs[0]) {
                    pos.x--;
                    pos.y++;
                    this.transform.eulerAngles = new Vector3(0, -45, 0);
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
                    this.transform.eulerAngles = new Vector3(0, 45, 0);
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
                    this.transform.eulerAngles = new Vector3(0, 225, 0);
                }
                break;
            case 7:
                if (moveDirs[6]) {
                    pos.y--;
                    this.transform.eulerAngles = new Vector3(0, 180, 0);
                }
                break;
            case 8:
                if (moveDirs[7]) {
                    pos.x++;
                    pos.y--;
                    this.transform.eulerAngles = new Vector3(0, 135, 0);
                }
                break;
        }
        player.CheckFogOfWar();
        CheckDirs();
        SetTileUnit();
        if (movesLeft <= 0) {
            EndTurn();
        }
    }

    public void SetTileUnit() {
        TileScript currentTile = gridScript.grid[pos.x, pos.y].tileScript;
        if (currentTile.SetUnit(this)) {
            meshObject.SetActive(false);
            City currentCity = currentTile.gameObject.GetComponent<City>();
            if (!currentCity.isOwned) {
                currentCity.GetOwned(this.player);
                player.playerCities.Add(currentCity);
            }
        } else {
            meshObject.SetActive(true);
        }
    }

    public void ToggleSleep() {
        if (!isSleeping) {
            isSleeping = true;
            EndTurn();
        } else {
            isSleeping = false;
            StartTurn();
        }
    }

    public void Later() {
        player.NextUnit(this, true);
    }

    public void Done() {
        EndTurn();
    }
}

public enum UnitMoveType { Land, Air, Sea }
