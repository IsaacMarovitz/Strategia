using UnityEngine;

public class Fighter : Unit {

    public int fuel;
    public int maxFuel;
    public int fuelPerMove;

    public override void NewDay(Player _player) {
        base.NewDay(_player);

        fuel = maxFuel;
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
                        if (tiles[i].tileType == TileType.City || tiles[i].tileType == TileType.CostalCity) {
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

        if (gridScript.grid[pos.x, pos.y].tileType == TileType.City || gridScript.grid[pos.x, pos.y].tileType == TileType.CostalCity) {
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
}