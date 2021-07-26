// Invisible unless immediatley adjacent
public class Submarine : Unit {
    public override void Start() {
        base.Start();
        unitType = UnitType.Submarine;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 1f, 0f, 0.34f, 0.8f, 0.8f };
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
                    returnMoveStatus = TileMoveStatus.Transport;
                } else {
                    returnMoveStatus = TileMoveStatus.Blocked;
                }
            } else {
                returnMoveStatus = TileMoveStatus.Attack;
            }
        } 

        return returnMoveStatus;
    }
}