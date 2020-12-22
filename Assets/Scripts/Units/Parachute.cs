using UnityEngine;

public class Parachute : Unit {

    public GameObject unitPrefab;

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

    public void DeployArmy() {
        Army army = GameObject.Instantiate(unitPrefab, Vector3.zero, Quaternion.identity).GetComponent<Army>();
        army.SetPos(pos);
        Die();
        gridScript.grid[pos.x, pos.y].unitOnTile = army;
        player.playerUnits.Add(army);
        if (isInCity) {
            oldCity.AddUnit(army);
        }
        GameObject.Destroy(this.gameObject);
    }
}