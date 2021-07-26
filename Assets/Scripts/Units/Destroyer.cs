public class Destroyer : Unit {
    public override void Start() {
        base.Start();
        unitType = UnitType.Destroyer;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0.3f, 0f, 0.25f, 0f, 1f, 0.34f, 1f, 0.34f, 0.1f };
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
}