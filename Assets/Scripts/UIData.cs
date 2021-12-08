public static class UIData {
    private static Unit _currentUnit;
    private static City _currentCity;
    private static Tile _mouseOverTile;

    public static Unit currentUnit {
        get {
            return _currentUnit;
        }

        private set {
            _currentUnit = value;
        }
    }

    public static City currentCity {
        get {
            return _currentCity;
        }

        private set {
            _currentCity = value;
        }
    }

    public static Tile mouseOverTile {
        get {
            return _mouseOverTile;
        }

        private set {
            _mouseOverTile = value;
        }
    }


    public static void SetUnit(Unit unit, bool smoothMove = true) {
        currentCity = null;
        DelegateManager.cityDeselectedDelegate?.Invoke();

        currentUnit = unit;
        if (unit != null) {
            if (GameManager.Instance != null) {
                GameManager.Instance.cameraController.Focus(GridUtilities.TileToWorldPos(unit.pos), smoothMove);
            }
            DelegateManager.unitSelectedDelegate?.Invoke(unit);
        } else {
            DelegateManager.unitDeselectedDelegate?.Invoke();
        }
    }

    public static void SetCity(City city, bool smoothMove = true) {
        currentUnit = null;
        DelegateManager.unitDeselectedDelegate?.Invoke();

        currentCity = city;
        if (city != null) {
            if (GameManager.Instance != null) {
                GameManager.Instance.cameraController.Focus(GridUtilities.TileToWorldPos(city.pos), smoothMove);
            }
            DelegateManager.citySelectedDelegate?.Invoke(city);
        } else {
            DelegateManager.cityDeselectedDelegate?.Invoke();
        }
    }

    public static void SetMouseOverTile(Tile tile) {
        if (mouseOverTile == tile) { return; }
        
        mouseOverTile = tile;
        if (tile != null) {
            DelegateManager.mouseOverTileSelectedDelegate?.Invoke(tile);
        } else {
            DelegateManager.mouseOverTileDeselectedDelegate?.Invoke();
        }
    }
}