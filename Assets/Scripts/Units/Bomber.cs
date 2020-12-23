using UnityEngine;
using System.Collections.Generic;

public class Bomber : Unit {

    public int blastRadius = 5;

    public override void Start() {
        unitType = UnitType.Bomber;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    }

    public override void CheckDirs() {
        base.CheckDirs();

        for (int i = 0; i < tiles.Length; i++) {
            if (tiles[i] == null) {
                moveDirs[i] = TileMoveStatus.Blocked;
            } else if (tiles[i].tileType == TileType.Mountains) {
                moveDirs[i] = TileMoveStatus.Blocked;
            } else {
                moveDirs[i] = TileMoveStatus.Move;
            }
        }

        if (turnStage == TurnStage.Started) {
            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i].unitOnTile != null) {
                    moveDirs[i] = TileMoveStatus.Blocked;
                } else {
                    if (tiles[i].tileType == TileType.City || tiles[i].tileType == TileType.CostalCity) {
                        City city = tiles[i].gameObject.GetComponent<City>();
                        if (!player.playerCities.Contains(city)) {
                            moveDirs[i] = TileMoveStatus.Blocked;
                        }
                    }
                }
            }
        }
    }

    public void Detonate() {
        List<Tile> tilesInRange = GridUtilities.RadialSearch(gridScript.grid, pos, blastRadius);
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
        GameObject.Destroy(this.gameObject);
    }
}