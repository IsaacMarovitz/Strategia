using UnityEngine;

public class Parachute : Unit {

    public GameObject unitPrefab;
    public int fuel;
    public int maxFuel;
    public int fuelPerMove;

    public override void Start() {
        base.Start();
        unitType = UnitType.Parachute;
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

    public void DeployArmy() {
        Army army = GameObject.Instantiate(unitPrefab, Vector3.zero, Quaternion.identity).GetComponent<Army>();
        army.SetPos(pos);
        army.gameObject.transform.parent = this.gameObject.transform.parent;
        army.player = player;
        Die();
        GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile = army;
        player.playerUnits.Add(army);
        if (isInCity) {
            oldCity.AddUnit(army);
            army.isInCity = true;
            army.mainMesh.SetActive(false);
            army.oldCity = oldCity;
        }
        GameObject.Destroy(this.gameObject);
    }
}