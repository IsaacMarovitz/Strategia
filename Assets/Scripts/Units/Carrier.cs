using UnityEngine;
using System.Collections.Generic;

public class Carrier : Unit, ITransport {

    public UnitType unitOnTransportType { get { return _unitOnTransportType; } }
    public List<Unit> unitsOnTransport { get { return _unitsOnTransport; } set { _unitsOnTransport = value; } }
    public int maxNumberOfUnits { get { return _maxNumberOfUnits; } }
    public bool isTransportFull { get { return _isTransportFull; } }

    public const UnitType _unitOnTransportType = UnitType.Fighter;
    public List<Unit> _unitsOnTransport;
    public const int _maxNumberOfUnits = 6;
    public bool _isTransportFull {
        get {
            if (unitsOnTransport.Count >= maxNumberOfUnits) {
                return true;
            } else {
                return false;
            }
        }
    }

    public override void Start() {
        base.Start();
        unitType = UnitType.Carrier;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0.5f, 0.1f, 0f, 0.25f, 0f };
    }

    public override void Update() {
        base.Update();
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
                returnMoveStatus = TileMoveStatus.Blocked;
            } else {
                returnMoveStatus = TileMoveStatus.Attack;
            }
        } 

        return returnMoveStatus;
    }

    public override void Die() {
        base.Die();
        foreach (var figher in unitsOnTransport) {
            figher.Die();
            GameObject.Destroy(figher.gameObject);
        }
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);
        foreach (var fighter in unitsOnTransport) {
            fighter.pos = pos;
        }
    }
}