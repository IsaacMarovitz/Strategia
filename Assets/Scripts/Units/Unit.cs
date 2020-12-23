using UnityEngine;
using UnityEngine.VFX;

public class Unit : MonoBehaviour {

    public Vector2Int pos;
    public float yOffset;
    public int health;
    public int maxHealth;
    public int moves;
    public int maxMoves;
    public int damageAmount;
    public TileMoveStatus[] moveDirs = new TileMoveStatus[8];
    public TurnStage turnStage = TurnStage.Waiting;
    public float[] damagePercentages = new float[9];
    public UnitType unitType;
    public City oldCity;
    public bool isInCity;
    public GameObject mainMesh;

    public Strategia.TileGrid gridScript;
    public VisualEffect sleepEffect;

    protected Player player;

    public void Awake() {
        // Replace this later with something a lot more modular
        mainMesh = this.gameObject.transform.GetChild(0).gameObject;
        sleepEffect = this.gameObject.transform.GetComponentInChildren<VisualEffect>();
    }

    public virtual void Start() {
        sleepEffect.enabled = false;
        moves = maxMoves;
        health = maxHealth;
        gridScript.grid[pos.x, pos.y].unitOnTile = this;
    }

    public void Update() {
        SetPos(pos);
        if (GameManager.Instance.GetCurrentPlayer().fogOfWarMatrix[pos.x, pos.y] != 1f) {
            mainMesh.SetActive(false);
        } else if (!isInCity) {
            mainMesh.SetActive(true);
        }
        if (gridScript.grid[pos.x, pos.y].tileType == TileType.City || gridScript.grid[pos.x, pos.y].tileType == TileType.CostalCity) {
            mainMesh.SetActive(false);
        }
        CheckDirs();
    }

    public virtual void NewDay(Player _player) {
        player = _player;
        turnStage = TurnStage.Waiting;
        moves = maxMoves;
    }

    public void StartTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn started");
        turnStage = TurnStage.Started;
        if (turnStage == TurnStage.Sleeping) {
            EndTurn();
            return;
        }
    }

    public void EndTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn complete");
        player.NextUnit(this, false);
        turnStage = TurnStage.Complete;
    }

    public void ToggleSleep() {
        if (turnStage != TurnStage.Sleeping) {
            turnStage = TurnStage.Sleeping;
            sleepEffect.enabled = true;
            sleepEffect.Play();
            EndTurn();
        } else {
            turnStage = TurnStage.Started;
            sleepEffect.Stop();
            player.AddToUnitQueue(this);
            StartTurn();
        }
    }

    public void Later() {
        player.NextUnit(this, true);
    }

    public void Attack(Vector2Int unitPos) {
        Unit unitToAttack = gridScript.grid[pos.x, pos.y].unitOnTile;
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

    public virtual void CheckDirs() {
        if (moves <= 0) {
            turnStage = TurnStage.Complete;
            return;
        }

        // Each unit implements it's own logic here
    }

    public virtual void Move(int dir) {
        moves--;
        gridScript.grid[pos.x, pos.y].unitOnTile = null;
        int[] rotationOffset = new int[8] { -45, 0, 45, -90, 90, 255, 180, 135 };
        Vector2Int offset = Vector2Int.zero;
        if (dir == 1 || dir == 4 || dir == 6) {
            offset.x--;
        } else if (dir == 3 || dir == 5 || dir == 8) {
            offset.x++;
        }
        if (dir <= 3) {
            offset.y++;
        } else if (dir >= 6) {
            offset.y--;
        }
        if (moveDirs[dir - 1] == TileMoveStatus.Move) {
            pos += offset;
            this.transform.eulerAngles = new Vector3(0, rotationOffset[dir - 1], 0);
        } else if (moveDirs[dir - 1] == TileMoveStatus.Attack) {
            Attack(pos += offset);
        }

        if (oldCity != null) { oldCity = null; };

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

        gridScript.grid[pos.x, pos.y].unitOnTile = this;
        if (moves <= 0) {
            EndTurn();
        }
        player.UpdateFogOfWar();
    }

    public void SetColor(Color color) {
        mainMesh.GetComponent<MeshRenderer>().material.color = color;
    }

    public void SetPos(Vector2Int _pos) {
        transform.position = new Vector3(_pos.x * gridScript.tileWidth, yOffset, _pos.y * gridScript.tileHeight);
    }

    public void Die() {
        player.playerUnits.Remove(this);
        gridScript.grid[pos.x, pos.y].unitOnTile = null;
        if (isInCity) {
            oldCity.RemoveUnit(this);
        }
    }
}

public enum TurnStage { Waiting, Started, Complete, Sleeping }
public enum TileMoveStatus { Move, Transport, Attack, Blocked }
public enum UnitType { Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship }