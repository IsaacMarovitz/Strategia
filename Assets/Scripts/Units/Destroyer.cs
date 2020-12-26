using UnityEngine;

public class Destroyer : Unit {
    public override void Start() {
        base.Start();
        unitType = UnitType.Destroyer;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0.3f, 0f, 0.25f, 0f, 1f, 0.34f, 1f, 0.34f, 0.1f };
    }

    public override void CheckDirs() {
        base.CheckDirs();

        Tile[] tiles = GridUtilities.DiagonalCheck(pos);
        for (int i = 0; i < tiles.Length; i++) {
            if (tiles[i] == null) {
                moveDirs[i] = TileMoveStatus.Blocked;
            } else if (tiles[i].tileType == TileType.Sea) {
                moveDirs[i] = TileMoveStatus.Move;
            } else if (tiles[i].tileType == TileType.CostalCity) {
                moveDirs[i] = TileMoveStatus.Move;
            } else {
                moveDirs[i] = TileMoveStatus.Blocked;
            }
        }

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