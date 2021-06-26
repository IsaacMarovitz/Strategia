using UnityEngine;
using System.Collections.Generic;

public class Transport : Unit {

    public List<Tank> armiesOnTransport;
    public bool isTransportFull;

    public override void Start() {
        base.Start();
        unitType = UnitType.Transport;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    }

    public override void Update() {
        base.Update();
        if (armiesOnTransport.Count >= 6) {
            isTransportFull = true;
        } else {
            isTransportFull = false;
        }
    }

    public override TileMoveStatus CheckDir(Tile tile) {
        TileMoveStatus returnMoveStatus = base.CheckDir(tile);

        if (turnStage == TurnStage.Started) {
            if (tile.unitOnTile != null) {
                returnMoveStatus = TileMoveStatus.Blocked;
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
        foreach (var army in armiesOnTransport) {
            army.Die();
            GameObject.Destroy(army.gameObject);
        }
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);
        foreach (var army in armiesOnTransport) {
            army.pos = pos;
        }
    }
}