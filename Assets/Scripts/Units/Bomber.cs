using UnityEngine;
using System.Collections.Generic;

public class Bomber : Unit, ICustomButton, IFuel {

    public string CustomButtonName { get { return "Detonate"; } }

    public int fuel { get { return _fuel; } set { _fuel = value; } }
    public int maxFuel { get { return _maxFuel; } set { _maxFuel = value; } }
    public int fuelPerMove { get { return _fuelPerMove; } set { _fuelPerMove = value; } }

    public int _fuel;
    public int _maxFuel;
    public int _fuelPerMove;

    public int blastRadius = 2;

    public override void Start() {
        base.Start();
        unitType = UnitType.Bomber;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    }

    public override void NewDay(Player _player) {
        base.NewDay(_player);

        fuel = maxFuel;
    }

    public override TileMoveStatus CheckDir(Tile tile) {
        TileMoveStatus returnMoveStatus = base.CheckDir(tile);

        if (tile.isCityTile) {
            City city = tile.gameObject.GetComponent<City>();
            if (!player.playerCities.Contains(city)) {
                returnMoveStatus = TileMoveStatus.Blocked;
            }
        }

        if (tile.unitOnTile != null) {
            returnMoveStatus = TileMoveStatus.Blocked;
        }

        return returnMoveStatus;
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);

        if (currentTile.isCityTile) {
            fuel = maxFuel;
        } else {
            fuel -= fuelPerMove;
        }

        if (fuel <= 0) {
            Debug.Log($"<b>{this.gameObject.name}:</b> Ran out of fuel and crashed!");
            Die();
            GameObject.Destroy(this.gameObject);
        }
    }

    public void Detonate() {
        List<Tile> tilesInRange = GridUtilities.RadialSearch(pos, blastRadius);
        foreach (var tile in tilesInRange) {
            if (tile.isCityTile) {
                tile.gameObject.GetComponent<City>().Nuke();
            }

            if (tile.unitOnTile != null) {
                Unit unitToKill = tile.unitOnTile;
                unitToKill.Die();
                GameObject.Destroy(unitToKill.gameObject);
            }
        }
        Debug.Log($"<b>{this.gameObject.name}:</b> Detonated!");
        Die();
        if (gameManager.GetCurrentPlayer().HasDied()) {
            gameManager.GetCurrentPlayer().TurnComplete();
        }
        GameObject.Destroy(this.gameObject);
    }

    public void CustomButton() {
        Detonate();
    }
}