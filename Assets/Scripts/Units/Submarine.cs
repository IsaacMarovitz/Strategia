public class Submarine : Unit {
    public override void Start() {
        base.Start();
        unitType = UnitType.Submarine;
        // Set damage percentages in order of Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 1f, 0f, 0.34f, 0.8f, 0.8f };
    }

    public override void OnFogOfWarUpdate(Player currentPlayer) {
        if (path != null) {
            for (int i = 0; i < path.Count; i++) {
                if (player.fogOfWarMatrix[path[i].pos.x, path[i].pos.y] != FogOfWarState.Hidden) {
                    if (CheckDir(path[i]) == TileMoveStatus.Blocked) {
                        path.RemoveRange(i, path.Count - i);
                        break;
                    }
                }
            }
        }

        if (currentPlayer == player) {
            if (currentTile.isCityTile) {
                unitAppearenceManager.Hide();
                instantiatedSleepEffect?.SetActive(false);
            } else {
                unitAppearenceManager.Show();
                instantiatedSleepEffect?.SetActive(true);
            }
        } else {
            if (currentPlayer.fogOfWarMatrix[pos.x, pos.y] != FogOfWarState.Visible) {
                unitAppearenceManager.Hide();
                instantiatedSleepEffect?.SetActive(false);
            } else {
                Tile[] tiles = GridUtilities.DiagonalCheck(pos);
                bool isVisible = false;

                foreach (var tile in tiles) {
                    if (tile.unitOnTile != null) {
                        if (tile.unitOnTile.player == currentPlayer) {
                            isVisible = true;
                        }
                    }
                    if (tile.isCityTile) {
                        City tileCity = tile.gameObject.GetComponent<City>();
                        if (tileCity != null) {
                            if (tileCity.unitsInCity.Count > 0) {
                                isVisible = true;
                            }
                        }
                    }
                }

                if (isVisible) {
                    unitAppearenceManager.Show();
                    instantiatedSleepEffect?.SetActive(true);
                } else {
                    unitAppearenceManager.Hide();
                    instantiatedSleepEffect?.SetActive(false);
                }
            }
        }
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