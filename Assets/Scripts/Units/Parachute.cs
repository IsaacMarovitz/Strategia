using UnityEngine;

public class Parachute : Unit, ICustomButton, IFuel {

    public GameObject unitPrefab;
    public int fuel { get; set; }
    public int maxFuel;
    public int fuelPerMove;
    public string CustomButtonName { get { return "Deploy"; } }

    public override void Start() {
        base.Start();
        unitType = UnitType.Parachute;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    }

    public override void NewDay(Player _player) {
        base.NewDay(_player);

        fuel = maxFuel;
    }

    public override TileMoveStatus CheckDir(Tile tile) {
        TileMoveStatus returnMoveStatus = base.CheckDir(tile);

        if (turnStage == TurnStage.Started) {
            if (tile.unitOnTile != null) {
                returnMoveStatus = TileMoveStatus.Blocked;
            } else {
                if (tile.tileType == TileType.City || tile.tileType == TileType.CostalCity) {
                    City city = tile.gameObject.GetComponent<City>();
                    if (!player.playerCities.Contains(city)) {
                        returnMoveStatus = TileMoveStatus.Blocked;
                    }
                }
            }
        }

        return returnMoveStatus;
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);

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
        if (GameManager.Instance.grid.grid[pos.x, pos.y].tileType != TileType.Sea || GameManager.Instance.grid.grid[pos.x, pos.y].tileType != TileType.Mountains || GameManager.Instance.grid.grid[pos.x, pos.y].tileType != TileType.Trees) {
            Tank army = GameObject.Instantiate(unitPrefab, Vector3.zero, Quaternion.identity).GetComponent<Tank>();
            army.pos = pos;
            army.gameObject.transform.parent = this.gameObject.transform.parent;
            army.gameObject.transform.rotation = this.gameObject.transform.rotation;
            army.player = player;
            Die();
            GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile = army;
            player.AddUnit(army);
            int i = player.unitQueue.FindIndex(a => a == this);
            if (i >= 0) {
                player.unitQueue[i] = army;
            }
            if (isInCity) {
                army.isInCity = true;
                army.mainMesh.SetActive(false);
                army.oldCity = oldCity;
                oldCity.RemoveUnit(this);
                oldCity.AddUnit(army);
            }
            UIData.Instance.currentUnit = army;
            Debug.Log($"<b>{this.gameObject.name}:</b> Deployed army!");
            GameObject.Destroy(this.gameObject);
        } else {
            Debug.Log($"<b>{this.gameObject.name}:</b> Invalid deploy location!");
        }
    }

    public void CustomButton() {
        DeployArmy();
    }
}