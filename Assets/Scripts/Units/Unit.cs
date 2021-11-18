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
    public UnitTurnStage unitTurnStage = UnitTurnStage.Waiting;
    public float[] damagePercentages = new float[9];
    public UnitType unitType;
    public City oldCity;
    public GameObject sleepEffectPrefab;
    public GameObject damageIndicatorPrefab;
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

    private bool hasBeenInitalised = false;

    public virtual void Start() {
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
            if (instantiatedSleepEffect != null) {
                instantiatedSleepEffect.SetActive(false);
            }
        } else {
            if (currentTile.isCityTile) {
                unitAppearanceManager.Hide();
                if (instantiatedSleepEffect != null) {
                instantiatedSleepEffect.SetActive(false);
            }
            } else {
                unitAppearanceManager.Show();
                if (instantiatedSleepEffect != null) {
                instantiatedSleepEffect.SetActive(true);
            }
            }
        }
    }

    public virtual void NewDay(Player _player) {
        player = _player;
        if (unitTurnStage != UnitTurnStage.Sleeping) {
            unitTurnStage = UnitTurnStage.Waiting;
        }
        if (!hasBeenInitalised) {
            health = maxHealth;
            if (!currentTile.isCityTile) {
                currentTile.unitOnTile = this;
            }
            hasBeenInitalised = true;
        }
        moves = maxMoves;
    }

    public void StartTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn started");
        if (unitTurnStage == UnitTurnStage.Sleeping) {
            EndTurn();
            return;
        } else if (path != null) {
            if (path.Count > 0) {
                unitTurnStage = UnitTurnStage.PathSet;
                EndTurn();
                return;
            } else {
                unitTurnStage = UnitTurnStage.Started;
            }
        } else {
            unitTurnStage = UnitTurnStage.Started;
        }
        DelegateManager.unitTurnStartDelegate?.Invoke(this);
    }

    public void EndTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn complete");
        player.NextUnit(this, false);
        DelegateManager.unitActionDelegate?.Invoke();
    }

    public void ToggleSleep() {
        if (unitTurnStage != UnitTurnStage.Sleeping) {
            unitTurnStage = UnitTurnStage.Sleeping;
            if (!currentTile.isCityTile) {
                instantiatedSleepEffect = GameObject.Instantiate(sleepEffectPrefab, this.transform.position, Quaternion.identity);
                instantiatedSleepEffect.transform.parent = this.transform;
            }

            if (player.playerTurnStage != PlayerTurnStage.Complete) {
                EndTurn();
            }
        } else {
            if (instantiatedSleepEffect != null) {
                GameObject.Destroy(instantiatedSleepEffect);
            }
            
            if (player.playerTurnStage != PlayerTurnStage.Complete) {
                unitTurnStage = UnitTurnStage.Started;
                player.unitQueue.Add(this);
                StartTurn();
            } else {
                unitTurnStage = UnitTurnStage.Complete;
            }
        }

        DelegateManager.unitActionDelegate?.Invoke();
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

        DelegateManager.unitActionDelegate?.Invoke();
    }

    public void TakeDamage(Unit unit) {
        int damage = Mathf.RoundToInt(maxHealth * unit.damagePercentages[(int)unitType]);
        health -= damage;
        GameObject instantiatedIndicator = GameObject.Instantiate(damageIndicatorPrefab, transform.position, Quaternion.identity);
        instantiatedIndicator.GetComponent<DamageIndicator>().IndicateDamage(damage);
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

        if (unitTurnStage == UnitTurnStage.PathSet) {
            if (path.Count <= 0) {
                if (moves <= 0) {
                    unitTurnStage = UnitTurnStage.Complete;
                } else {
                    unitTurnStage = UnitTurnStage.Started;
                }
            }
        } else {
            if (moves <= 0) {
                if (path.Count > 0) {
                    unitTurnStage = UnitTurnStage.PathSet;
                } else {
                    unitTurnStage = UnitTurnStage.Complete;
                }
                EndTurn();
            }
        }
        
        player.UpdateFogOfWar();
        DelegateManager.unitMoveDelegate?.Invoke(this);
        DelegateManager.unitActionDelegate?.Invoke();
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
        DelegateManager.unitActionDelegate?.Invoke();
    }

    public virtual void Die() {
        player.playerUnits.Remove(this);
        player.unitQueue.Remove(this);
        currentTile.unitOnTile = null;
        if (currentTile.isCityTile) {
            oldCity.RemoveUnit(this);
        }
        DelegateManager.unitActionDelegate?.Invoke();
    }
}

public enum UnitTurnStage { Waiting, Started, Complete, Sleeping, PathSet }
public enum TileMoveStatus { Move, Transport, Attack, Blocked }
public enum UnitType { Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship }