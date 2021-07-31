using UnityEngine;

public class Parachute : Unit, ICustomButton, IFuel {

    public GameObject unitPrefab;
    public int fuel { get; set; }
    public int maxFuel { get; set; }
    public int fuelPerMove { get; set; }
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

        if (tile.isCityTile) {
            City city = tile.gameObject.GetComponent<City>();
            if (!player.playerCities.Contains(city)) {
                returnMoveStatus = TileMoveStatus.Blocked;
            }
        }

        if (tile.unitOnTile != null) {
            returnMoveStatus = TileMoveStatus.Blocked;
        }


        return returnMoveStatus;
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);

        if (currentTile.isCityTile) {
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

    public void DeployTank() {
        if (currentTile.tileType != TileType.Sea || currentTile.tileType != TileType.Mountains || currentTile.tileType != TileType.Trees) {
            Tank tank = GameObject.Instantiate(unitPrefab, Vector3.zero, Quaternion.identity).GetComponent<Tank>();
            tank.pos = pos;
            tank.gameObject.transform.parent = this.gameObject.transform.parent;
            tank.gameObject.transform.rotation = this.gameObject.transform.rotation;
            tank.player = player;
            Die();
            currentTile.unitOnTile = tank;
            player.AddUnit(tank);
            int i = player.unitQueue.FindIndex(a => a == this);
            if (i >= 0) {
                player.unitQueue[i] = tank;
            }
            if (currentTile.isCityTile) {
                tank.mainMesh.SetActive(false);
                tank.oldCity = oldCity;
                oldCity.RemoveUnit(this);
                oldCity.AddUnit(tank);
            }
            UIData.Instance.currentUnit = tank;
            Debug.Log($"<b>{this.gameObject.name}:</b> Deployed tank!");
            GameObject.Destroy(this.gameObject);
        } else {
            Debug.Log($"<b>{this.gameObject.name}:</b> Invalid deploy location!");
        }
    }

    public void CustomButton() {
        DeployTank();
    }
}