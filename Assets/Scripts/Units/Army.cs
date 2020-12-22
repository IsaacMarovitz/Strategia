public class Army : Unit {

    public bool isMoveDistanceReduced;
    public int reducedMoveDistance;

    public override void NewDay(Player _player) {
        base.NewDay(_player);

        if (isMoveDistanceReduced) {
            moves -= reducedMoveDistance;
        }
    }

    public override void CheckDirs() {
        base.CheckDirs();

        for (int i = 0; i < tiles.Length; i++) {
            if (tiles[i] == null) {
                moveDirs[i] = TileMoveStatus.Blocked;
            } else if (tiles[i].tileType == TileType.Sea) {
                if (tiles[i].unitOnTile != null) {
                    if (tiles[i].unitOnTile.GetType() == typeof(Transport)) {
                        moveDirs[i] = TileMoveStatus.Transport;
                    }
                }
            } else if (tiles[i].tileType == TileType.Trees) {
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
                }
            }
        }
    }

    public override void Move(int dir) {
        base.Move(dir);

        if (gridScript.grid[pos.x, pos.y].tileType == TileType.Swamp) {
            isMoveDistanceReduced = true;
            moves -= reducedMoveDistance;
        } else {
            isMoveDistanceReduced = false;
        }
    }
}