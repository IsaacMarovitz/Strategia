using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;
using System.Collections;

public class Unit : MonoBehaviour {

    public UnitMoveType moveType;
    public Vector2Int pos;
    public int moveDistance;
    public bool moveDistanceReduced;
    public int moveDistanceReductionFactor = 4;
    public int maxHealth;
    public int health;
    public bool hasFuel;
    public int maxFuel;
    public int fuel;
    public MeshRenderer[] meshes;

    public Strategia.TileGrid gridScript;
    public VisualEffect sleepEffect;

    public int movesLeft;
    public MoveType[] moveDirs;

    public bool isSleeping = false;
    public bool turnStarted = false;
    public bool turnComplete = false;

    private GameObject meshObject;
    private Player player;
    private City oldCity;
    private GameObject mainMesh;
    private bool isInCity = true;

    // Move direction go from left to right, top to bottom
    // E.G. Left and Up = 1, Up = 2, Right and Up = 3 etc...

    public void NewDay(Player _player) {
        player = _player;
        turnStarted = false;
        turnComplete = false;
    }

    public void SetColor(Color color) {
        for (int i = 0; i < meshes.Length; i++) {
            meshes[i].material.color = color;
        }
    }

    public void StartTurn() {
        Debug.Log($"<b>{this.gameObject.name}</b>: Turn started");
        turnStarted = true;
        turnComplete = false;
        movesLeft = moveDistance;
        if (moveDistanceReduced) {
            movesLeft -= moveDistanceReductionFactor;
        }
        if (isSleeping) {
            EndTurn();
            return;
        }
    }

    public void EndTurn() {
        Debug.Log($"<b>{this.gameObject.name}</b>: Turn Complete");
        player.NextUnit(this, false);
        turnComplete = true;
    }

    public void Update() {
        transform.position = new Vector3(pos.x * gridScript.tileWidth, transform.position.y, pos.y * gridScript.tileHeight);
        if (GameManager.Instance.GetCurrentPlayer().fogOfWarMatrix[pos.x, pos.y] != 1f) {
            mainMesh.SetActive(false);
        } else if (!isInCity) {
            mainMesh.SetActive(true);
        }
        CheckDirs();
    }

    public void Start() {
        moveDirs = new MoveType[8];
        mainMesh = this.transform.GetChild(0).gameObject;
        movesLeft = moveDistance;
        meshObject = gameObject.transform.GetChild(0).gameObject;
        health = maxHealth;
        gridScript.grid[pos.x, pos.y].unitOnTile = this;
        City city = gridScript.grid[pos.x, pos.y].gameObject.GetComponent<City>();
        city.AddUnit(this);
        oldCity = city;
        isInCity = true;
        mainMesh.SetActive(false);
    }

    public void Attack(int posX, int posY) {
        Unit unitToAttack = gridScript.grid[posX, posY].unitOnTile;
        if (unitToAttack != null) {
            Debug.Log($"<b>{this.gameObject.name}:</b> Attacking {unitToAttack.gameObject.name}!");
            unitToAttack.TakeDamage(10);
        } else {
            Debug.LogWarning($"<b>{this.gameObject.name}:</b> Could not find unit!");
        }
    }

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            Debug.Log($"<b>{this.gameObject.name}:</b> Took {damage} damage, and died!");
            player.playerUnits.Remove(this);
            gridScript.grid[pos.x, pos.y].unitOnTile = null;
            if (isInCity) {
                oldCity.RemoveUnit(this);
            }
            GameObject.Destroy(this.gameObject);
        } else {
            Debug.Log($"<b>{this.gameObject.name}:</b> Took {damage} damage! Current health: {health}");
        }
    }

    public void CheckDirs() {
        if (movesLeft <= 0) {
            turnComplete = true;
            moveDirs = new MoveType[] { MoveType.No, MoveType.No, MoveType.No, MoveType.No, MoveType.No, MoveType.No, MoveType.No, MoveType.No };
        } else {
            Tile[] tiles = GridUtilities.DiagonalCheck(gridScript.grid, gridScript.width, gridScript.height, pos);

            switch (moveType) {
                case UnitMoveType.Air:
                    for (int i = 0; i < tiles.Length; i++) {
                        if (tiles[i] == null) {
                            moveDirs[i] = MoveType.No;
                        } else if (tiles[i].tileType == TileType.Mountains) {
                            moveDirs[i] = MoveType.No;
                        } else {
                            moveDirs[i] = MoveType.Move;
                        }
                    }
                    break;
                case UnitMoveType.Land:
                    for (int i = 0; i < tiles.Length; i++) {
                        if (tiles[i] == null) {
                            moveDirs[i] = MoveType.No;
                        } else if ((tiles[i].tileType == TileType.Sea) || (tiles[i].tileType == TileType.Trees)) {
                            moveDirs[i] = MoveType.No;
                        } else {
                            moveDirs[i] = MoveType.Move;
                        }
                    }
                    break;
                case UnitMoveType.Sea:
                    for (int i = 0; i < tiles.Length; i++) {
                        if (tiles[i] == null) {
                            moveDirs[i] = MoveType.No;
                        } else if (tiles[i].tileType != TileType.Sea) {
                            moveDirs[i] = MoveType.No;
                        } else {
                            moveDirs[i] = MoveType.Move;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (turnStarted && !turnComplete) {
                for (int i = 0; i < tiles.Length; i++) {
                    if (tiles[i].unitOnTile != null) {
                        if (tiles[i].tileType == TileType.City || tiles[i].tileType == TileType.CostalCity) {
                            City city = tiles[i].gameObject.GetComponent<City>();
                            if (!player.playerCities.Contains(city)) {
                                moveDirs[i] = MoveType.Attack;
                            }
                        } else {
                            if (player.playerUnits.Contains(tiles[i].unitOnTile)) {
                                moveDirs[i] = MoveType.No;
                            } else {
                                moveDirs[i] = MoveType.Attack;
                            }
                        }
                    } else {
                        if (tiles[i].tileType == TileType.City || tiles[i].tileType == TileType.CostalCity) {
                            City city = tiles[i].gameObject.GetComponent<City>();
                            if (!player.playerCities.Contains(city) && moveType != UnitMoveType.Land) {
                                moveDirs[i] = MoveType.No;
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(5);
        EndTurn();
    }

    public void Move(int dir) {
        movesLeft--;
        gridScript.grid[pos.x, pos.y].unitOnTile = null;
        switch (dir) {
            case 1:
                if (moveDirs[0] == MoveType.Move) {
                    pos.x--;
                    pos.y++;
                    this.transform.eulerAngles = new Vector3(0, -45, 0);
                } else if (moveDirs[0] == MoveType.Attack) {
                    Attack(pos.x - 1, pos.y + 1);
                }
                break;
            case 2:
                if (moveDirs[1] == MoveType.Move) {
                    pos.y++;
                    this.transform.eulerAngles = new Vector3(0, 0, 0);
                } else if (moveDirs[1] == MoveType.Attack) {
                    Attack(pos.x, pos.y + 1);
                }
                break;
            case 3:
                if (moveDirs[2] == MoveType.Move) {
                    pos.x++;
                    pos.y++;
                    this.transform.eulerAngles = new Vector3(0, 45, 0);
                } else if (moveDirs[2] == MoveType.Attack) {
                    Attack(pos.x + 1, pos.x + 1);
                }
                break;
            case 4:
                if (moveDirs[3] == MoveType.Move) {
                    pos.x--;
                    this.transform.eulerAngles = new Vector3(0, -90, 0);
                } else if (moveDirs[3] == MoveType.Attack) {
                    Attack(pos.x - 1, pos.y);
                }
                break;
            case 5:
                if (moveDirs[4] == MoveType.Move) {
                    pos.x++;
                    this.transform.eulerAngles = new Vector3(0, 90, 0);
                } else if (moveDirs[4] == MoveType.Attack) {
                    Attack(pos.x + 1, pos.y);
                }
                break;
            case 6:
                if (moveDirs[5] == MoveType.Move) {
                    pos.x--;
                    pos.y--;
                    this.transform.eulerAngles = new Vector3(0, 225, 0);
                } else if (moveDirs[5] == MoveType.Attack) {
                    Attack(pos.x - 1, pos.y - 1);
                }
                break;
            case 7:
                if (moveDirs[6] == MoveType.Move) {
                    pos.y--;
                    this.transform.eulerAngles = new Vector3(0, 180, 0);
                } else if (moveDirs[6] == MoveType.Attack) {
                    Attack(pos.x, pos.y - 1);
                }
                break;
            case 8:
                if (moveDirs[7] == MoveType.Move) {
                    pos.x++;
                    pos.y--;
                    this.transform.eulerAngles = new Vector3(0, 135, 0);
                } else if (moveDirs[7] == MoveType.Attack) {
                    Attack(pos.x + 1, pos.y - 1);
                }
                break;
        }
        if (oldCity != null) {
            oldCity.RemoveUnit(this);
            oldCity = null;
        }
        if (gridScript.grid[pos.x, pos.y].tileType == TileType.City || gridScript.grid[pos.x, pos.y].tileType == TileType.CostalCity) {
            City city = gridScript.grid[pos.x, pos.y].gameObject.GetComponent<City>();
            city.GetOwned(player);
            city.AddUnit(this);
            oldCity = city;
            isInCity = true;
            mainMesh.SetActive(false);
        } else {
            isInCity = false;
            mainMesh.SetActive(true);
        }
        if (gridScript.grid[pos.x, pos.y].tileType == TileType.Swamp && moveType == UnitMoveType.Land) {
            moveDistanceReduced = true;
            movesLeft -= moveDistanceReductionFactor;
        } else {
            moveDistanceReduced = false;
        }
        gridScript.grid[pos.x, pos.y].unitOnTile = this;
        player.UpdateFogOfWar();
        if (movesLeft <= 0) {
            EndTurn();
        }
    }

    public void ToggleSleep() {
        if (!isSleeping) {
            isSleeping = true;
            sleepEffect.Play();
            EndTurn();
        } else {
            isSleeping = false;
            sleepEffect.Stop();
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
public enum MoveType { Move, Attack, No }