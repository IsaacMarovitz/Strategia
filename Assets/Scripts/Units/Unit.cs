using UnityEngine;
using System.Collections.Generic;

public class Unit : TurnBehaviour {

    public Vector2Int pos;
    public Tile currentTile { get { return grid[pos.x, pos.y]; } }
    public float yOffset;
    public int health;
    public int maxHealth;
    public int moves;
    public int maxMoves;
    public TurnStage turnStage = TurnStage.Waiting;
    public float[] damagePercentages = new float[9];
    public UnitType unitType;
    public City oldCity;
    public GameObject sleepEffectPrefab;
    public Sprite unitIcon;
    public Player player;
    public List<TileType> blockedTileTypes;
    public UnitMoveUI unitMoveUI;
    public UnitAppearanceManager unitAppearanceManager;
    public bool infiniteAttack = false;

    [HideInInspector]
    public List<Tile> path { get; private set; }

    protected bool pathWasSetThisTurn;
    protected GameObject instantiatedSleepEffect;

    public virtual void Start() {
        moves = maxMoves;
        health = maxHealth;
        if (!currentTile.isCityTile) {
            currentTile.unitOnTile = this;
        }
        path = new List<Tile>();
    }

    public virtual void Update() {
        SetPos(pos);
    }

    public override void OnFogOfWarUpdate(Player currentPlayer) {
        if (path != null) {
            for (int i = 0; i < path.Count; i++) {
                if (player.fogOfWarMatrix[path[i].pos.x, path[i].pos.y] != FogOfWarState.Hidden) {
                    if (CheckDir(path[i]) == TileMoveStatus.Blocked) {
                        path.RemoveRange(i, path.Count - i);
                        break;
                    }
                }
            }
        }

        if (currentPlayer.fogOfWarMatrix[pos.x, pos.y] != FogOfWarState.Visible) {
            unitAppearanceManager.Hide();
            instantiatedSleepEffect?.SetActive(false);
        } else {
            if (currentTile.isCityTile) {
                unitAppearanceManager.Hide();
                instantiatedSleepEffect?.SetActive(false);
            } else {
                unitAppearanceManager.Show();
                instantiatedSleepEffect?.SetActive(true);
            }
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
            if (!currentTile.isCityTile) {
                instantiatedSleepEffect = GameObject.Instantiate(sleepEffectPrefab, this.transform.position, Quaternion.identity);
                instantiatedSleepEffect.transform.parent = this.transform;
            }

            if (!player.turnCompleted) {
                EndTurn();
            }
        } else {
            if (instantiatedSleepEffect != null) {
                GameObject.Destroy(instantiatedSleepEffect);
            }
            
            if (!player.turnCompleted) {
                turnStage = TurnStage.Started;
                player.unitQueue.Add(this);
                StartTurn();
            } else {
                turnStage = TurnStage.Complete;
            }
        }
    }

    public void Later() {
        player.NextUnit(this, true);
    }

    public void Attack(Vector2Int unitPos) {
        Unit unitToAttack = null;
        if (grid[unitPos.x, unitPos.y].isCityTile) {
            City tileCity = grid[unitPos.x, unitPos.y].gameObject.GetComponent<City>();
            if (tileCity != null) {
                if (tileCity.unitsInCity.Count > 0) {
                    unitToAttack = tileCity.unitsInCity[0];
                }
            }
        } else {
            unitToAttack = grid[unitPos.x, unitPos.y].unitOnTile;
        }
        if (unitToAttack != null) {
            Debug.Log($"<b>{this.gameObject.name}:</b> Attacking {unitToAttack.gameObject.name}");
            unitToAttack.TakeDamage(this);
            if (!infiniteAttack && !GameManager.Instance.infiniteAttack) {
                moves = 0;
            }
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
                if (path[i] != currentTile) {
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
            if (path[i] != currentTile) {
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
        if (tileMoveStatus == TileMoveStatus.Move || tileMoveStatus == TileMoveStatus.Transport) {
            TransportCheck();
            this.gameObject.transform.LookAt(tileToMoveTo.gameObject.transform.position, Vector3.up);
            this.gameObject.transform.eulerAngles = new Vector3(0, this.gameObject.transform.eulerAngles.y, 0);
            pos = tileToMoveTo.pos;
            path.Remove(tileToMoveTo);

            TransportMove(tileToMoveTo, tileMoveStatus);

            if (oldCity != null) {
                oldCity.RemoveUnit(this);
                oldCity = null;
                unitAppearanceManager.Show();
            }

            if (currentTile.isCityTile) {
                City city = currentTile.gameObject.GetComponent<City>();
                city.GetOwned(player);
                city.AddUnit(this);
                oldCity = city;
                unitAppearanceManager.Hide();
            } else {
                unitAppearanceManager.Show();
                currentTile.unitOnTile = this;
            }
        } else if (tileMoveStatus == TileMoveStatus.Attack) {
            if (pathWasSetThisTurn) {
                Attack(tileToMoveTo.pos);
                path.Clear();
            } else {
                this.gameObject.transform.LookAt(tileToMoveTo.gameObject.transform.position, Vector3.up);
                this.gameObject.transform.eulerAngles = new Vector3(0, this.gameObject.transform.eulerAngles.y, 0);
                path.Clear();
                currentTile.unitOnTile = this;
                return;
            }
        } else if (tileMoveStatus == TileMoveStatus.Blocked) {
            path.Clear();
            currentTile.unitOnTile = this;
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
        currentTile.unitOnTile = null;
    }

    public virtual void TransportMove(Tile tileToMoveTo, TileMoveStatus tileMoveStatus) { }

    public void SetColor(Player player) {
        unitAppearanceManager.UpdateColor(player);
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
        currentTile.unitOnTile = null;
        if (currentTile.isCityTile) {
            oldCity.RemoveUnit(this);
        }
    }
}

public enum TurnStage { Waiting, Started, Complete, Sleeping, PathSet }
public enum TileMoveStatus { Move, Transport, Attack, Blocked }
public enum UnitType { Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship }