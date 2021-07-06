using UnityEngine;

public class Tank : Unit {

    public bool isMoveDistanceReduced;
    public bool isOnTransport = false;
    public int reducedMoveDistance;

    public override void Start() {
        base.Start();
        unitType = UnitType.Tank;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0.34f, 0f, 0.25f, 0f, 0.2f, 0.1f, 0.3f, 0.1f, 0f };
    }

    public override void Update() {
        base.Update();
        if (isOnTransport) {
            isInCity = false;
            mainMesh.SetActive(false);
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
        
        if (tile.tileType == TileType.City || tile.tileType == TileType.CostalCity) {
            City city = tile.gameObject.GetComponent<City>();
            if (!player.playerCities.Contains(city)) {
                if (city.unitsInCity.Count > 0 && !isOnTransport) {
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
                if (!isOnTransport) {
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
        if (GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.Swamp) {
            isMoveDistanceReduced = true;
            moves -= reducedMoveDistance;
        } else {
            isMoveDistanceReduced = false;
        }
    }

    public override void TransportCheck() {
        if (isOnTransport) {
            isOnTransport = false;
            mainMesh.SetActive(true);
            ((Transport)GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile).armiesOnTransport.Remove(this);
        } else {
            GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile = null;
        }
    }

    public override void TransportMove(Tile tileToMoveTo, TileMoveStatus tileMoveStatus) {
        base.TransportMove(tileToMoveTo, tileMoveStatus);
        if (tileMoveStatus == TileMoveStatus.Transport) {
            pos = tileToMoveTo.pos;
            isOnTransport = true;
            ((Transport)GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile).armiesOnTransport.Add(this);
        }
    }
}