using UnityEngine;
using System.Collections.Generic;

public class Bomber : Unit {

    public int blastRadius = 2;
    public int fuel;
    public int maxFuel;
    public int fuelPerMove;

    public override void Start() {
        base.Start();
        unitType = UnitType.Bomber;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    }

    public override void NewDay(Player _player) {
        base.NewDay(_player);

        fuel = maxFuel;
    }

    public override void CheckDirs() {
        base.CheckDirs();

        Tile[] tiles = GridUtilities.DiagonalCheck(pos);
        
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

    public override void Move(int dir) {
        base.Move(dir);

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
        GameObject.Destroy(this.gameObject);
    }
}