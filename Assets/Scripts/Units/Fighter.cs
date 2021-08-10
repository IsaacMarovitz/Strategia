using UnityEngine;

public class Fighter : Unit, IFuel {

    public int fuel { get { return _fuel; } set { _fuel = value; } }
    public int maxFuel { get { return _maxFuel; } set { _maxFuel = value; } }
    public int fuelPerMove { get { return _fuelPerMove; } set { _fuelPerMove = value; } }

    public int _fuel;
    public int _maxFuel;
    public int _fuelPerMove;

    public Carrier carrier;

    public override void Start() {
        base.Start();
        unitType = UnitType.Fighter;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0.5f, 1f, 0.34f, 1f, 0.5f, 0.25f, 0.5f, 0.2f, 0.1f };
    }

    public override void Update() {
        base.Update();
        if (carrier != null) {
            mainMesh.SetActive(false);
        }
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
                if (city.unitsInCity.Count > 0) {
                    returnMoveStatus = TileMoveStatus.Attack;
                } else {
                    returnMoveStatus = TileMoveStatus.Blocked;
                }
            }
        }

        if (tile.unitOnTile != null) {
            if (player.playerUnits.Contains(tile.unitOnTile)) {
                if (tile.unitOnTile.GetType() == typeof(Carrier)) {
                    if (!tile.unitOnTile.GetComponent<Carrier>().isTransportFull) {
                        returnMoveStatus = TileMoveStatus.Transport;
                    } else {
                        returnMoveStatus = TileMoveStatus.Blocked;
                    }
                } else {
                    returnMoveStatus = TileMoveStatus.Blocked;
                }
            } else {
                returnMoveStatus = TileMoveStatus.Attack;
            }
        }

        return returnMoveStatus;
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);

        if (currentTile.isCityTile || carrier != null) {
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
        if (carrier != null) {
            carrier.unitsOnTransport.Remove(this);
            carrier = null;
            mainMesh.SetActive(true);
        } else {
            currentTile.unitOnTile = null;
        }
    }

    public override void TransportMove(Tile tileToMoveTo, TileMoveStatus tileMoveStatus) {
        base.TransportMove(tileToMoveTo, tileMoveStatus);
        if (tileMoveStatus == TileMoveStatus.Transport) {
            try {
                carrier = (Carrier)currentTile.unitOnTile;
                carrier.unitsOnTransport.Add(this);
                UIData.SetUnit(carrier);
                EndTurn();
            }
            catch {
                Debug.LogError($"<b>{this.gameObject.name}:</b> Failed to get carrier at ({tileToMoveTo.pos.x}, {tileToMoveTo.pos.y})!");
            }
        }
    }
}