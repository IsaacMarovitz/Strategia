using System;
using UnityEngine;

namespace Strategia {
    public class SpawnUnitCommand : IDebugCommand {
        public string commandId { get { return "spawn_unit"; } }
        public string commandDescription { get { return "Spawns a unit at the given coordinate."; } }
        public string commandFormat { get { return "spawn_unit x: <int> y: <int> <UnitType>"; } }

        public bool Process(string[] args, DebugConsole debugConsole) {
            if (args.Length > 2) {
                if (int.TryParse(args[0], out int x)) {
                    if (x > GameManager.Instance.grid.width || x < 0) {
                        debugConsole.PrintError($"x-coordinate '{x}' out of bounds!");
                        return false;
                    }
                } else {
                    debugConsole.PrintError($"Failed to parse x-coordinate '{args[0]}'!");
                    return false;
                }
                if (int.TryParse(args[1], out int y)) {
                    if (y > GameManager.Instance.grid.height || y < 0) {
                        debugConsole.PrintError($"y-coordinate '{y}' out of bounds!");
                        return false;
                    }
                } else {
                    debugConsole.PrintError($"Failed to parse y-coordinate '{args[1]}'!");
                    return false;
                }
                if (!Enum.TryParse<UnitType>(args[2], out UnitType unitType)) {
                    debugConsole.PrintError($"Failed to parse UnitType '{args[2]}'!");
                    return false;
                }
                if (GameManager.Instance.unitInfo.allUnits[(int)unitType].blockedTileTypes.Contains(GameManager.Instance.grid.grid[x, y].tileType)) {
                    debugConsole.PrintError($"Unit of type '{unitType}' cannot be spawned on tiles of type '{GameManager.Instance.grid.grid[x, y].tileType}'!");
                    return false;
                }
                if (GameManager.Instance.grid.grid[x, y].unitOnTile != null) {
                    debugConsole.PrintError($"Tile at ({x}, {y}) already occupied!");
                    return false;
                }
                if (GameManager.Instance.grid.grid[x, y].tileType == TileType.City || GameManager.Instance.grid.grid[x, y].tileType == TileType.CostalCity) {
                    if (!GameManager.Instance.GetCurrentPlayer().playerCities.Contains(GameManager.Instance.grid.grid[x, y].gameObject.GetComponent<City>())) {
                        debugConsole.PrintError($"Player does not own city at ({x}, {y})!");
                        return false;
                    }
                }
                GameManager.Instance.GetCurrentPlayer().SpawnUnit(new Vector2Int(x, y), unitType);
                return true;
            } else {
                debugConsole.PrintError($"Missing arguments!");
                return false;
            }
        }
    }
}