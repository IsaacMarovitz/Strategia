using UnityEngine;
using System.Collections.Generic;

public class Transport : Unit, ITransport {

    public UnitType unitOnTransportType { get { return _unitOnTransportType; } }
    public List<Unit> unitsOnTransport { get { return _unitsOnTransport; } set { _unitsOnTransport = value; } }
    public int maxNumberOfUnits { get { return _maxNumberOfUnits; } }
    public bool isTransportFull { get { return _isTransportFull; } }

    public const UnitType _unitOnTransportType = UnitType.Tank;
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
        unitType = UnitType.Transport;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
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

    public override void Die() {
        base.Die();
        foreach (var tank in unitsOnTransport) {
            tank.Die();
            GameObject.Destroy(tank.gameObject);
        }
    }

    public override void PerformMove(Tile tileToMoveTo) {
        base.PerformMove(tileToMoveTo);
        foreach (var tank in unitsOnTransport) {
            tank.pos = pos;
        }
    }
}