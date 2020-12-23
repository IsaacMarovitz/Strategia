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

    public Strategia.TileGrid gridScript;
    public VisualEffect sleepEffect;

    protected GameObject mainMesh;
    protected Tile[] tiles;
    protected Player player;
    protected City oldCity;
    protected bool isInCity;

    public virtual void Start() {
        sleepEffect.enabled = false;
        moves = maxMoves;
        health = maxHealth;
        // Get player main mesh?
        // Set unitOnTile in gridScript to this unit
        // Add unit to starting city (Perhaps do this on GameManager instead)
    }

    public void Update() {
        SetPos(pos);
        if (GameManager.Instance.GetCurrentPlayer().fogOfWarMatrix[pos.x, pos.y] != 1f) {
            mainMesh.SetActive(false);
        } else if (!isInCity) {
            mainMesh.SetActive(true);
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
            // If unit is sleeping, move on
        }
    }

    public void EndTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn complete");
        // player.NextUnit(this, false);
        turnStage = TurnStage.Complete;
    }

    public void ToggleSleep() {

    }

    public void Later() {

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

        tiles = GridUtilities.DiagonalCheck(gridScript.grid, gridScript.width, gridScript.height, pos);

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
            offset.y--;
        } else if (dir >= 6) {
            offset.y++;
        }
        if (moveDirs[dir - 1] == TileMoveStatus.Move) {
            pos += offset;
            this.transform.eulerAngles = new Vector3(0, rotationOffset[dir - 1], 0);
        } else if (moveDirs[dir - 1] == TileMoveStatus.Attack) {
            Attack(pos += offset);
        }
        // Remove from old city if old city != null
        // Add to new city if on city
        // Set grid script unit on tile to this
        player.UpdateFogOfWar();
    }

    public void SetColor(Color color) {
        // Set color
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