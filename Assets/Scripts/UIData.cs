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


    public static void SetUnit(Unit unit) {
        currentUnit = unit;
        if (unit != null) {
            DelegateManager.unitSelectedDelegate?.Invoke(unit);
        } else {
            DelegateManager.unitDeselectedDelegate?.Invoke();
        }
    }

    public static void SetCity(City city) {
        currentCity = city;
        if (city != null) {
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