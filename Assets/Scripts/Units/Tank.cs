using UnityEngine;

public class Tank : Unit {

    public bool isMoveDistanceReduced;
    public int reducedMoveDistance;

    public Transport transport;

    public override void Start() {
        base.Start();
        unitType = UnitType.Tank;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0.34f, 0f, 0.25f, 0f, 0.2f, 0.1f, 0.3f, 0.1f, 0f };
    }

    public override void Update() {
        base.Update();
        if (transport != null) {
            unitAppearanceManager.Hide();
        }
    }

    public override void NewDay(Player _player) {
        base.NewDay(_player);

        if (isMoveDistanceReduced) {
            moves -= reducedMoveDistance;
        }
    }

    public override TileMoveStatus CheckDir(Tile tile) {
        TileMoveStatus returnMoveStatus = base.CheckDir(tile);

        if (tile.isCityTile) {
            City city = tile.gameObject.GetComponent<City>();
            if (!player.playerCities.Contains(city)) {
                if (city.unitsInCity.Count > 0 && transport == null) {
                    returnMoveStatus = TileMoveStatus.Attack;
                }
            }
        }

        if (tile.unitOnTile != null) {
            if (player.playerUnits.Contains(tile.unitOnTile)) {
                if (tile.unitOnTile.GetType() == typeof(Transport)) {
                    if (!tile.unitOnTile.GetComponent<Transport>().isTransportFull) {
                        returnMoveStatus = TileMoveStatus.Transport;
                    } else {
                        returnMoveStatus = TileMoveStatus.Blocked;
                    }
                } else {
                    returnMoveStatus = TileMoveStatus.Blocked;
                }
            } else {
                if (transport == null) {
                    returnMoveStatus = TileMoveStatus.Attack;
                } else {
                    returnMoveStatus = TileMoveStatus.Blocked;
                }
            }
        }

        return returnMoveStatus;
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);
        if (currentTile.tileType == TileType.Swamp) {
            isMoveDistanceReduced = true;
            moves -= reducedMoveDistance;
        } else {
            isMoveDistanceReduced = false;
        }
    }

    public override void TransportCheck() {
        if (transport != null) {
            transport.unitsOnTransport.Remove(this);
            transport = null;
            unitAppearanceManager.Show();
        } else {
            currentTile.unitOnTile = null;
        }
    }

    public override void TransportMove(Tile tileToMoveTo, TileMoveStatus tileMoveStatus) {
        base.TransportMove(tileToMoveTo, tileMoveStatus);
        if (tileMoveStatus == TileMoveStatus.Transport) {
            try {
                transport = (Transport)currentTile.unitOnTile;
                transport.unitsOnTransport.Add(this);
                UIData.SetUnit(transport);
                EndTurn();
            } 
            catch {
                Debug.LogError($"<b>{this.gameObject.name}:</b> Failed to get transport at ({tileToMoveTo.pos.x}, {tileToMoveTo.pos.y})!");
            }
        }
    }
}