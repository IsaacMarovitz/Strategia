using UnityEngine;
using System.Collections.Generic;

public class Carrier : Unit {

    public List<Fighter> fightersOnCarrier;

    public override void Start() {
        base.Start();
        unitType = UnitType.Carrier;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0.5f, 0.1f, 0f, 0.25f, 0f };
    }

    public override void Update() {
        base.Update();
    }

    public override TileMoveStatus CheckDir(Tile tile) {
        TileMoveStatus returnMoveStatus = base.CheckDir(tile);

        if (turnStage == TurnStage.Started) {
            if (tile.unitOnTile != null) {
                if (tile.tileType == TileType.CostalCity) {
                    City city = tile.gameObject.GetComponent<City>();
                    if (!player.playerCities.Contains(city)) {
                        returnMoveStatus = TileMoveStatus.Attack;
                    }
                } else {
                    if (player.playerUnits.Contains(tile.unitOnTile)) {
                        returnMoveStatus = TileMoveStatus.Blocked;
                    } else {
                        returnMoveStatus = TileMoveStatus.Attack;
                    }
                }
            } else {
                if (tile.tileType == TileType.CostalCity) {
                    City city = tile.gameObject.GetComponent<City>();
                    if (!player.playerCities.Contains(city)) {
                        returnMoveStatus = TileMoveStatus.Blocked;
                    }
                }
            }
        }

        return returnMoveStatus;
    }

    public override void Die() {
        base.Die();
        foreach (var figher in fightersOnCarrier) {
            figher.Die();
            GameObject.Destroy(figher.gameObject);
        }
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);
        foreach (var fighter in fightersOnCarrier) {
            fighter.pos = pos;
        }
    }
}