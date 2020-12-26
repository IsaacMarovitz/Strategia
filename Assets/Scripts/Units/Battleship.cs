using UnityEngine;

public class Battleship : Unit {
    public override void Start() {
        base.Start();
        unitType = UnitType.Battleship;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 1f, 0f, 0.5f, 0f, 1f, 1f, 0.2f, 0.8f, 0.25f };
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
}