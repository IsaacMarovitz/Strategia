using UnityEngine;
using System.Collections.Generic;

public class Bomber : Unit, ICustomButton, IFuel {

    public int blastRadius = 2;
    public int fuel { get; set; }
    public int maxFuel { get; set; }
    public int fuelPerMove { get; set; }
    public string CustomButtonName { get { return "Detonate"; } }

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

        if (turnStage == TurnStage.Started) {
            if (tile.unitOnTile != null) {
                returnMoveStatus = TileMoveStatus.Blocked;
            } else {
                if (tile.tileType == TileType.City || tile.tileType == TileType.CostalCity) {
                    City city = tile.gameObject.GetComponent<City>();
                    if (!player.playerCities.Contains(city)) {
                        returnMoveStatus = TileMoveStatus.Blocked;
                    }
                }
            }
        }

        return returnMoveStatus;
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);

        if (GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.City || GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.CostalCity) {
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
            if (tile.tileType == TileType.City || tile.tileType == TileType.CostalCity) {
                tile.gameObject.GetComponent<City>().Nuke();
            }

            if (tile.unitOnTile != null) {
                tile.unitOnTile.Die();
                GameObject.Destroy(tile.unitOnTile.gameObject);
            }
        }
        Debug.Log($"<b>{this.gameObject.name}:</b> Detonated!");
        Die();
        if (GameManager.Instance.GetCurrentPlayer().HasDied()) {
            GameManager.Instance.GetCurrentPlayer().TurnComplete();
        }
        GameObject.Destroy(this.gameObject);
    }

    public void CustomButton() {
        Detonate();
    }
}