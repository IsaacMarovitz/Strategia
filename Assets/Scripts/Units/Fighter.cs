using UnityEngine;

public class Fighter : Unit, IFuel {

    public int fuel { get; set; }
    public int maxFuel;
    public int fuelPerMove;
    public bool isOnCarrier = false;

    public override void Start() {
        base.Start();
        unitType = UnitType.Fighter;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0.5f, 1f, 0.34f, 1f, 0.5f, 0.25f, 0.5f, 0.2f, 0.1f };
    }

    public override void Update() {
        base.Update();
        if (isOnCarrier) {
            isInCity = false;
            mainMesh.SetActive(false);
        }
    }


    public override void NewDay(Player _player) {
        base.NewDay(_player);

        fuel = maxFuel;
    }

    public override TileMoveStatus CheckDir(Tile tile) {
        TileMoveStatus returnMoveStatus = base.CheckDir(tile);

        if (turnStage == TurnStage.Started) {
            if (tile.unitOnTile != null) {
                if (tile.tileType == TileType.City || tile.tileType == TileType.CostalCity) {
                    City city = tile.gameObject.GetComponent<City>();
                    if (!player.playerCities.Contains(city)) {
                        returnMoveStatus = TileMoveStatus.Attack;
                    }
                } else {
                    if (player.playerUnits.Contains(tile.unitOnTile)) {
                        if (tile.unitOnTile.GetType() == typeof(Carrier)) {
                            returnMoveStatus = TileMoveStatus.Transport;
                        } else {
                            returnMoveStatus = TileMoveStatus.Blocked;
                        }
                    } else {
                        returnMoveStatus = TileMoveStatus.Attack;
                    }
                }
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

    public override void TransportCheck() {
        base.TransportCheck();
        if (isOnCarrier) {
            isOnCarrier = false;
            mainMesh.SetActive(true);
            ((Carrier)GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile).fightersOnCarrier.Remove(this);
        } else {
            GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile = null;
        }
    }

    public override void TransportMove(Tile tileToMoveTo, TileMoveStatus tileMoveStatus) {
        base.TransportMove(tileToMoveTo, tileMoveStatus);
        if (tileMoveStatus == TileMoveStatus.Transport) {
            pos += tileToMoveTo.pos;
            isOnCarrier = true;
            ((Carrier)GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile).fightersOnCarrier.Add(this);
            fuel = maxFuel;
        }
    }
}