using UnityEngine;
using System.Collections.Generic;

public class Unit : TurnBehaviour {

    public Vector2Int pos;
    public float yOffset;
    public int health;
    public int maxHealth;
    public int moves;
    public int maxMoves;
    public int damageAmount;
    public TurnStage turnStage = TurnStage.Waiting;
    public float[] damagePercentages = new float[9];
    public UnitType unitType;
    public City oldCity;
    public bool isInCity;
    public GameObject mainMesh;
    public GameObject sleepEffectPrefab;
    public Sprite unitIcon;
    public Player player;
    public List<TileType> blockedTileTypes;

    [HideInInspector]
    public List<Tile> path { get; private set; }

    protected bool pathWasSetThisTurn;

    private GameObject instantiatedSleepEffect;
    public LineRenderer lineRenderer;

    public override void Awake() {
        base.Awake();

        // Replace this later with something a lot more modular
        mainMesh = this.gameObject.transform.GetChild(0).gameObject;
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    public virtual void Start() {
        moves = maxMoves;
        health = maxHealth;
        if (!tileGrid.grid[pos.x, pos.y].isCityTile) {
            tileGrid.grid[pos.x, pos.y].unitOnTile = this;
        }
        path = new List<Tile>();
    }

    public virtual void Update() {
        SetPos(pos);
        if (UIData.Instance.currentUnit != this && path.Count > 0 && lineRenderer != null) {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = path.Count;
            List<Tile> tempPath = new List<Tile>(path);
            tempPath.Insert(0, tileGrid.grid[pos.x, pos.y]);
            lineRenderer.SetPositions(GridUtilities.TilesToWorldPos(tempPath));
        } else {
            lineRenderer.enabled = false;
        }
    }

    public override void OnPlayerTurnStart(Player player) {
        if (player != this.player) {
            lineRenderer.gameObject.SetActive(false);
        }
    }

    public override void OnFogOfWarUpdate(Player player) {
        if (player.fogOfWarMatrix[pos.x, pos.y] != 1f) {
            mainMesh.SetActive(false);
            lineRenderer.gameObject.SetActive(false);
            instantiatedSleepEffect?.SetActive(false);
        } else if (!isInCity) {
            mainMesh.SetActive(true);
            lineRenderer.gameObject.SetActive(true);
            instantiatedSleepEffect?.SetActive(true);
        }
        if (tileGrid.grid[pos.x, pos.y].isCityTile) {
            mainMesh.SetActive(false);
            lineRenderer.gameObject.SetActive(false);
            instantiatedSleepEffect?.SetActive(false);
        }
    }

    public virtual void NewDay(Player _player) {
        player = _player;
        if (turnStage != TurnStage.Sleeping) {
            turnStage = TurnStage.Waiting;
        }
        moves = maxMoves;
    }

    public void StartTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn started");
        if (turnStage == TurnStage.Sleeping) {
            EndTurn();
            return;
        } else if (path != null) {
            if (path.Count > 0) {
                turnStage = TurnStage.PathSet;
                EndTurn();
                return;
            } else {
                turnStage = TurnStage.Started;
            }
        } else {
            turnStage = TurnStage.Started;
        }
    }

    public void EndTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn complete");
        player.NextUnit(this, false);
    }

    public void ToggleSleep() {
        if (turnStage != TurnStage.Sleeping) {
            turnStage = TurnStage.Sleeping;
            if (!tileGrid.grid[pos.x, pos.y].isCityTile) {
                instantiatedSleepEffect = GameObject.Instantiate(sleepEffectPrefab, this.transform.position, Quaternion.identity);
                instantiatedSleepEffect.transform.parent = this.transform;
            }
            EndTurn();
        } else {
            turnStage = TurnStage.Started;
            if (instantiatedSleepEffect != null) {
                GameObject.Destroy(instantiatedSleepEffect);
            }
            player.unitQueue.Add(this);
            StartTurn();
        }
    }

    public void Later() {
        player.NextUnit(this, true);
    }

    public void Attack(Vector2Int unitPos) {
        Unit unitToAttack = null;
        if (tileGrid.grid[unitPos.x, unitPos.y].isCityTile) {
            City tileCity = tileGrid.grid[unitPos.x, unitPos.y].gameObject.GetComponent<City>();
            if (tileCity != null) {
                if (tileCity.unitsInCity.Count > 0) {
                    unitToAttack = tileCity.unitsInCity[0];
                }
            }
        } else {
            unitToAttack = tileGrid.grid[unitPos.x, unitPos.y].unitOnTile;
        }
        if (unitToAttack != null) {
            Debug.Log($"<b>{this.gameObject.name}:</b> Attacking {unitToAttack.gameObject.name}");
            unitToAttack.TakeDamage(this);
        } else {
            Debug.LogWarning($"<b>{this.gameObject.name}:</b> Could not find unit to attack at {unitPos}!");
        }
    }

    public void TakeDamage(Unit unit) {
        int damage = Mathf.RoundToInt(maxHealth * unit.damagePercentages[(int)unitType]);
        health -= damage;
        if (health <= 0) {
            Debug.Log($"<b>{this.gameObject.name}:</b> Took {damage} damage, and died!");
            Die();
            GameObject.Destroy(this.gameObject);
        } else {
            Debug.Log($"<b>{this.gameObject.name}:</b> Took {damage} damage! Current health: {health}");
        }
    }

    public virtual TileMoveStatus CheckDir(Tile tile) {
        TileMoveStatus returnMoveStatus = new TileMoveStatus();
        if (blockedTileTypes.Contains(tile.tileType)) {
            returnMoveStatus = TileMoveStatus.Blocked;
        } else {
            returnMoveStatus = TileMoveStatus.Move;
        }
        return returnMoveStatus;
    }

    public void MoveAlongSetPath() {
        if (path != null) {
            for (int i = 0; i < path.Count; i++) {
                if (path[i] != tileGrid.grid[pos.x, pos.y]) {
                    if (path[i].unitOnTile != null) {
                        path = null;
                        return;
                    } else {
                        if (moves > 0) {
                            PerformMove(path[i]);
                            i--;
                        }
                    }
                }
            }
        }
    }

    public virtual void MoveAlongPath(List<Tile> newPath) {
        path = newPath;
        pathWasSetThisTurn = true;
        for (int i = 0; i < path.Count; i++) {
            if (path[i] != tileGrid.grid[pos.x, pos.y]) {
                if (moves > 0) {
                    PerformMove(path[i]);
                    i--;
                } else {
                    pathWasSetThisTurn = false;
                    EndTurn();
                }
            } else {
                path.RemoveAt(i);
                i--;
            }
        }
    }

    public virtual void PerformMove(Tile tileToMoveTo) {
        TileMoveStatus tileMoveStatus = CheckDir(tileToMoveTo);
        if (tileMoveStatus == TileMoveStatus.Move) {
            TransportCheck();
            this.gameObject.transform.LookAt(tileToMoveTo.gameObject.transform.position, Vector3.up);
            this.gameObject.transform.eulerAngles = new Vector3(0, this.gameObject.transform.eulerAngles.y, 0);
            pos = tileToMoveTo.pos;
            path.Remove(tileToMoveTo);

            TransportMove(tileToMoveTo, tileMoveStatus);

            if (oldCity != null) {
                oldCity.RemoveUnit(this);
                oldCity = null;
                isInCity = false;
                mainMesh.SetActive(true);
            }

            if (tileGrid.grid[pos.x, pos.y].isCityTile) {
                City city = tileGrid.grid[pos.x, pos.y].gameObject.GetComponent<City>();
                city.GetOwned(player);
                city.AddUnit(this);
                oldCity = city;
                isInCity = true;
                mainMesh.SetActive(false);
            } else {
                isInCity = false;
                mainMesh.SetActive(true);
                tileGrid.grid[pos.x, pos.y].unitOnTile = this;
            }
        } else if (tileMoveStatus == TileMoveStatus.Attack) {
            if (pathWasSetThisTurn) {
                Attack(tileToMoveTo.pos);
                path.Clear();
            } else {
                this.gameObject.transform.LookAt(tileToMoveTo.gameObject.transform.position, Vector3.up);
                this.gameObject.transform.eulerAngles = new Vector3(0, this.gameObject.transform.eulerAngles.y, 0);
                path.Clear();
                tileGrid.grid[pos.x, pos.y].unitOnTile = this;
                return;
            }
        } else if (tileMoveStatus == TileMoveStatus.Blocked) {
            path.Clear();
            tileGrid.grid[pos.x, pos.y].unitOnTile = this;
            return;
        }

        moves--;

        if (moves <= 0) {
            if (path.Count > 0) {
                turnStage = TurnStage.PathSet;
            } else {
                turnStage = TurnStage.Complete;
            }
            EndTurn();
        }
        player.UpdateFogOfWar();
        gameManager.OnUnitMove(this);
    }

    public virtual void TransportCheck() {
        tileGrid.grid[pos.x, pos.y].unitOnTile = null;
    }

    public virtual void TransportMove(Tile tileToMoveTo, TileMoveStatus tileMoveStatus) { }

    public void SetColor(Color color) {
        mainMesh.GetComponent<MeshRenderer>().material.color = color;
    }

    public void SetPos(Vector2Int _pos) {
        transform.position = GridUtilities.TileToWorldPos(_pos, yOffset);
    }

    public void UnsetPath() {
        path.Clear();
    }

    public virtual void Die() {
        player.playerUnits.Remove(this);
        player.unitQueue.Remove(this);
        tileGrid.grid[pos.x, pos.y].unitOnTile = null;
        if (isInCity) {
            oldCity.RemoveUnit(this);
        }
    }
}

public enum TurnStage { Waiting, Started, Complete, Sleeping, PathSet }
public enum TileMoveStatus { Move, Transport, Attack, Blocked }
public enum UnitType { Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship }