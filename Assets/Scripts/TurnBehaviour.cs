using UnityEngine;
using Strategia;

public class TurnBehaviour : MonoBehaviour {

    public GameManager gameManager => GameManager.Instance;
    public TileGrid tileGrid => gameManager.tileGrid;

    public virtual void Awake() {
        // gameManager.tick += Tick;
        gameManager.playerTurnStartDelegate += OnPlayerTurnStart;
        gameManager.unitTurnStartDelegate += OnUnitTurnStart;
        gameManager.unitMoveDelegate += OnUnitMove;
        gameManager.unitMoveDelegate += (Unit unit) => OnFogOfWarUpdate(unit.player);
        gameManager.playerTurnStartDelegate += OnFogOfWarUpdate;
        gameManager.fogOfWarUpdateDelegate += OnFogOfWarUpdate;
    }

    public virtual void OnDestroy() {
        // gameManager.tick -= Tick;
        gameManager.playerTurnStartDelegate -= OnPlayerTurnStart;
        gameManager.unitTurnStartDelegate -= OnUnitTurnStart;
        gameManager.unitMoveDelegate -= OnUnitMove;
        gameManager.unitMoveDelegate -= (Unit unit) => OnFogOfWarUpdate(unit.player);
        gameManager.playerTurnStartDelegate -= OnFogOfWarUpdate;
        gameManager.fogOfWarUpdateDelegate -= OnFogOfWarUpdate;
    }

    // Called when any delegate is invoked;
    // public virtual void Tick() {}

    // Called after a player has finished starting its turn
    public virtual void OnPlayerTurnStart(Player player) {}

    // Called after a unit has finished starting its turn
    public virtual void OnUnitTurnStart(Unit unit) {}

    // Called after a unit has moved to a new tile
    public virtual void OnUnitMove(Unit unit) {}

    // Called when unit moves and when player turn starts
    public virtual void OnFogOfWarUpdate(Player player) {}
}