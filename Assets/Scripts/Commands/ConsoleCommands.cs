using UnityEngine;
using System;

public static class ConsoleCommands {
    [DebugCommand("clear", "Clears the console.", "clear")]
    public static bool Clear(string[] args, DebugConsole debugConsole) {
        debugConsole.ClearConsole();
        return true;
    }

    [DebugCommand("enable_fog", "Enables or disables Fog of War for current player.", "enable_fog <bool>")]
    public static bool ClearFog(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            if (ConsoleCommands.ParseBool(args[0], out bool boolValue)) {
                GameManager.Instance.GetCurrentPlayer().RevealAllTiles(!boolValue);
                if (!boolValue) {
                    debugConsole.PrintSuccess($"Fog of War for Player {GameManager.Instance.currentPlayerIndex} disabled.");
                } else {
                    debugConsole.PrintSuccess($"Fog of War for Player {GameManager.Instance.currentPlayerIndex} enabled.");
                }
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    [DebugCommand("fast_prod", "Changes all units TTCs to 1 day.", "fast_prod <bool>")]
    public static bool FastProd(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            if (ConsoleCommands.ParseBool(args[0], out bool boolValue)) {
                GameManager.Instance.GetCurrentPlayer().RevealAllTiles(boolValue);
                if (boolValue) {
                    debugConsole.PrintSuccess("Set all unit TTCs to 1.");
                } else {
                    debugConsole.PrintSuccess("Reset all unit TTCs.");
                }
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }

    }

    [DebugCommand("help", "Shows list of available commands.", "help")]
    public static bool Help(string[] args, DebugConsole debugConsole) {
        debugConsole.PrintHelp();
        return true;
    }

    [DebugCommand("spawn_unit", "Spawns a unit at the given coordinate.", "spawn_unit x: <int> y: <int> <UnitType>")]
    public static bool SpawnUnit(string[] args, DebugConsole debugConsole) {
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

    [DebugCommand("test_with_num", "Test with debug console with num.", "test_with_num <num>")]
    public static bool TestWithNum(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            if (int.TryParse(args[0], out int intValue)) {
                debugConsole.PrintSuccess($"Test Num: {intValue}");
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    public static bool ParseBool(string input, out bool output) {
        output = false;
        if (bool.TryParse(input, out bool value)) {
            output = value;
            return true;
        } else {
            if (int.TryParse(input, out int valueInt)) {
                if (valueInt == 0) {
                    output = false;
                    return true;
                } else if (valueInt == 1) {
                    output = true;
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }
    }
}
