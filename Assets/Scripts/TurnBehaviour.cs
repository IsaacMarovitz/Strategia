using UnityEngine;
using Strategia;

public class TurnBehaviour : MonoBehaviour {

    public GameManager gameManager => GameManager.Instance;
    public TileGrid tileGrid => GameManager.Instance.tileGrid;
    public Tile[,] grid => GameManager.Instance.tileGrid.grid;

    public virtual void Awake() {
        DelegateManager.playerTurnWaitDelegate += OnPlayerTurnWait;
        DelegateManager.playerTurnStartDelegate += OnPlayerTurnStart;
        DelegateManager.playerTurnCompleteDelegate += OnPlayerTurnComplete;
        DelegateManager.playerTurnEndDelegate += OnPlayerTurnEnd;

        DelegateManager.unitMoveDelegate += OnUnitMove;
        DelegateManager.unitMoveDelegate += (Unit unit) => OnFogOfWarUpdate(unit.player);
        DelegateManager.unitActionDelegate += OnUnitAction;
        DelegateManager.playerTurnStartDelegate += OnFogOfWarUpdate;
        DelegateManager.fogOfWarUpdateDelegate += OnFogOfWarUpdate;

        DelegateManager.unitSelectedDelegate += OnUnitSelected;
        DelegateManager.unitDeselectedDelegate += OnUnitDeselected;

        DelegateManager.citySelectedDelegate += OnCitySelected;
        DelegateManager.cityDeselectedDelegate += OnCityDeselected;

        DelegateManager.mouseOverTileSelectedDelegate += OnMouseOverTileSelected;
        DelegateManager.mouseOverTileDeselectedDelegate += OnMouseOverTileDeselected;
    }

    public virtual void OnDestroy() {
        DelegateManager.playerTurnWaitDelegate -= OnPlayerTurnWait;
        DelegateManager.playerTurnStartDelegate -= OnPlayerTurnStart;
        DelegateManager.playerTurnCompleteDelegate -= OnPlayerTurnComplete;
        DelegateManager.playerTurnEndDelegate -= OnPlayerTurnEnd;

        DelegateManager.unitTurnStartDelegate -= OnUnitTurnStart;
        DelegateManager.unitMoveDelegate -= OnUnitMove;
        DelegateManager.unitMoveDelegate -= (Unit unit) => OnFogOfWarUpdate(unit.player);
        DelegateManager.unitActionDelegate -= OnUnitAction;
        DelegateManager.playerTurnStartDelegate -= OnFogOfWarUpdate;
        DelegateManager.fogOfWarUpdateDelegate -= OnFogOfWarUpdate;

        DelegateManager.unitSelectedDelegate -= OnUnitSelected;
        DelegateManager.unitDeselectedDelegate -= OnUnitDeselected;

        DelegateManager.citySelectedDelegate -= OnCitySelected;
        DelegateManager.cityDeselectedDelegate -= OnCityDeselected;

        DelegateManager.mouseOverTileSelectedDelegate -= OnMouseOverTileSelected;
        DelegateManager.mouseOverTileDeselectedDelegate -= OnMouseOverTileDeselected;
    }

    // Called after a player starts waiting
    public virtual void OnPlayerTurnWait(Player player) {}

    // Called after a player has finished starting its turn
    public virtual void OnPlayerTurnStart(Player player) {}

    // Called after a player complete's its turn
    public virtual void OnPlayerTurnComplete(Player player) {}

    // Called after a player had finished ending its turn
    public virtual void OnPlayerTurnEnd(Player player) {}

    // Called after a unit has finished starting its turn
    public virtual void OnUnitTurnStart(Unit unit) {}

    // Called after a unit has moved to a new tile
    public virtual void OnUnitMove(Unit unit) {}

    // Called after a unit performes any action
    public virtual void OnUnitAction() {}

    // Called when unit moves and when player turn starts
    public virtual void OnFogOfWarUpdate(Player player) {}

    // Called when a unit is selected
    public virtual void OnUnitSelected(Unit unit) {}

    // Called when a unit is deselected
    public virtual void OnUnitDeselected() {}

    // Called when a city is selected
    public virtual void OnCitySelected(City city) {}

    // Called when a city is deselected
    public virtual void OnCityDeselected() {}

    // Called when a mouse over tile is selected
    public virtual void OnMouseOverTileSelected(Tile tile) {}

    // Called when a mouse over tile is deselected
    public virtual void OnMouseOverTileDeselected() {}
}