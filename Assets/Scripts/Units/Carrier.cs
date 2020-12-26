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

    public override void CheckDirs() {
        base.CheckDirs();

        Tile[] tiles = GridUtilities.DiagonalCheck(pos);

        if (turnStage == TurnStage.Started) {
            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i].unitOnTile != null) {
                    if (tiles[i].tileType == TileType.CostalCity) {
                        City city = tiles[i].gameObject.GetComponent<City>();
                        if (!player.playerCities.Contains(city)) {
                            moveDirs[i] = TileMoveStatus.Attack;
                        }
                    } else {
                        if (player.playerUnits.Contains(tiles[i].unitOnTile)) {
                            moveDirs[i] = TileMoveStatus.Blocked;
                        } else {
                            moveDirs[i] = TileMoveStatus.Attack;
                        }
                    }
                } else {
                    if (tiles[i].tileType == TileType.CostalCity) {
                        City city = tiles[i].gameObject.GetComponent<City>();
                        if (!player.playerCities.Contains(city)) {
                            moveDirs[i] = TileMoveStatus.Blocked;
                        }
                    }
                }
            }
        }
    }

    public override void Die() {
        base.Die();
        foreach (var figher in fightersOnCarrier) {
            figher.Die();
            GameObject.Destroy(figher.gameObject);
        }
    }

    public override void Move(int dir) {
        base.Move(dir);
        foreach (var fighter in fightersOnCarrier) {
            fighter.pos = pos;
        }
    }
}