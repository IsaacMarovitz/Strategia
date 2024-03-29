using UnityEngine;
using System;

public static class ConsoleCommands {
    [DebugCommand("clear", "Clears the console.", "clear")]
    public static DebugConsole.DebugCommandCode Clear(string[] args, DebugConsole debugConsole) {
        debugConsole.ClearConsole();
        return DebugConsole.DebugCommandCode.Success;
    }

    [DebugCommand("say", "Say the input text in the console.", "say <string>")]
    public static DebugConsole.DebugCommandCode Say(string[] args, DebugConsole debugConsole) {
        string finalString = "";
        foreach (var text in args) {
            finalString += $"{text} ";
        }
        debugConsole.PrintString(finalString);
        return DebugConsole.DebugCommandCode.Success;
    }

    /*[DebugCommand("player_info", "Prints a palyer's info.", "player_info (optional) <int>")]
    public static DebugConsole.DebugCommandCode PlayerInfo(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            
            return DebugConsole.DebugCommandCode.Success;
        } else {
            return DebugConsole.DebugCommandCode.Success;
        }
    }*/

    [DebugCommand("enable_fog", "Enables or disables Fog of War for current player.", "enable_fog <bool>")]
    public static DebugConsole.DebugCommandCode ClearFog(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            if (ConsoleCommands.ParseBool(args[0], out bool boolValue)) {
                GameManager.Instance.GetCurrentPlayer().RevealAllTiles(!boolValue);
                DelegateManager.fogOfWarUpdateDelegate?.Invoke(GameManager.Instance.GetCurrentPlayer());
                if (!boolValue) {
                    debugConsole.PrintSuccess($"Fog of War for Player {GameManager.Instance.currentPlayerIndex} disabled.");
                } else {
                    debugConsole.PrintSuccess($"Fog of War for Player {GameManager.Instance.currentPlayerIndex} enabled.");
                }
                return DebugConsole.DebugCommandCode.Success;
            } else {
                return DebugConsole.DebugCommandCode.ParameterFailedParse;
            }
        } else {
            return DebugConsole.DebugCommandCode.MissingParameters;
        }
    }

    [DebugCommand("all_int_attack", "Enables or disables infinite attacking for all units.", "all_inf_attack <bool>")]
    public static DebugConsole.DebugCommandCode AllInfAttack(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            if (ConsoleCommands.ParseBool(args[0], out bool boolValue)) {
                GameManager.Instance.infiniteAttack = boolValue;
                if (boolValue) {
                    debugConsole.PrintSuccess("Enabled infinte attacking for all units.");
                } else {
                    debugConsole.PrintSuccess("Disabled infinte attacking for all units.");
                }
                return DebugConsole.DebugCommandCode.Success;
            } else {
                return DebugConsole.DebugCommandCode.ParameterOutOfRange;
            }
        } else {
            return DebugConsole.DebugCommandCode.MissingParameters;
        }
    }

    [DebugCommand("inf_attack", "Enables or disables infinite attacking for the current unit.", "inf_attack <bool>")]
    public static DebugConsole.DebugCommandCode InfAttack(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            if (ConsoleCommands.ParseBool(args[0], out bool boolValue)) {
                if (UIData.currentUnit != null) {
                    UIData.currentUnit.infiniteAttack = boolValue;
                    if (boolValue) {
                        debugConsole.PrintSuccess("Enabled infinte attacking for the current unit.");
                    } else {
                        debugConsole.PrintSuccess("Disabled infinte attacking for the current unit.");
                    }
                    return DebugConsole.DebugCommandCode.Success;
                } else {
                    debugConsole.PrintError("No unit currently selected.");
                    return DebugConsole.DebugCommandCode.ParameterOutOfRange;
                }
            } else {
                return DebugConsole.DebugCommandCode.ParameterFailedParse;
            }
        } else {
            return DebugConsole.DebugCommandCode.MissingParameters;
        }
    }

    [DebugCommand("set_num_moves", "Sets the number of moves left for the current unit.", "set_num_moves <int>")]
    public static DebugConsole.DebugCommandCode SetNumMoves(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            if (int.TryParse(args[0], out int intValue)) {
                if (UIData.currentUnit != null) {
                    if (intValue <= 0) {
                        debugConsole.PrintError("Number of moves must be greater than 0.");
                        return DebugConsole.DebugCommandCode.ParameterOutOfRange;
                    } else {
                        UIData.currentUnit.moves = intValue;
                        debugConsole.PrintSuccess($"Set current unit moves to {intValue}.");
                        DelegateManager.unitActionDelegate?.Invoke();
                        return DebugConsole.DebugCommandCode.Success;
                    }
                } else {
                    debugConsole.PrintError("No unit currently selected.");
                    return DebugConsole.DebugCommandCode.ParameterOutOfRange;
                }
            } else {
                return DebugConsole.DebugCommandCode.ParameterFailedParse;
            }
        } else {
            return DebugConsole.DebugCommandCode.MissingParameters;
        }
    }

    [DebugCommand("fast_prod", "Changes all units TTCs to 1 day.", "fast_prod <bool>")]
    public static DebugConsole.DebugCommandCode FastProd(string[] args, DebugConsole debugConsole) {
        if (args.Length > 0) {
            if (ConsoleCommands.ParseBool(args[0], out bool boolValue)) {
                GameManager.Instance.fastProdDelegate?.Invoke(boolValue);
                if (boolValue) {
                    debugConsole.PrintSuccess("Set all unit TTCs to 1.");
                } else {
                    debugConsole.PrintSuccess("Reset all unit TTCs.");
                }
                return DebugConsole.DebugCommandCode.Success;
            } else {
                return DebugConsole.DebugCommandCode.ParameterFailedParse;
            }
        } else {
            return DebugConsole.DebugCommandCode.MissingParameters;
        }
    }

    [DebugCommand("help", "Shows list of available commands.", "help")]
    public static DebugConsole.DebugCommandCode Help(string[] args, DebugConsole debugConsole) {
        debugConsole.PrintHelp();
        return DebugConsole.DebugCommandCode.Success;
    }

    [DebugCommand("spawn_unit", "Spawns a unit at the given coordinate.", "spawn_unit x: <int> y: <int> <UnitType>")]
    public static DebugConsole.DebugCommandCode SpawnUnit(string[] args, DebugConsole debugConsole) {
        if (args.Length > 2) {
            if (int.TryParse(args[0], out int x)) {
                if (x > GameManager.Instance.tileGrid.width || x < 0) {
                    debugConsole.PrintError($"x-coordinate '{x}' out of bounds!");
                    return DebugConsole.DebugCommandCode.ParameterOutOfRange;
                }
            } else {
                debugConsole.PrintError($"Failed to parse x-coordinate '{args[0]}'!");
                return DebugConsole.DebugCommandCode.ParameterOutOfRange;
            }
            if (int.TryParse(args[1], out int y)) {
                if (y > GameManager.Instance.tileGrid.height || y < 0) {
                    debugConsole.PrintError($"y-coordinate '{y}' out of bounds!");
                    return DebugConsole.DebugCommandCode.ParameterOutOfRange;
                }
            } else {
                debugConsole.PrintError($"Failed to parse y-coordinate '{args[1]}'!");
                return DebugConsole.DebugCommandCode.ParameterOutOfRange;
            }
            if (!Enum.TryParse<UnitType>(args[2], out UnitType unitType)) {
                debugConsole.PrintError($"Failed to parse UnitType '{args[2]}'!");
                return DebugConsole.DebugCommandCode.ParameterOutOfRange;
            }

            Unit unit = GameManager.Instance.GetUnitFromType(unitType);
            if (unit == null) { return DebugConsole.DebugCommandCode.ParameterOutOfRange; }

            if (unit.blockedTileTypes.Contains(GameManager.Instance.tileGrid.grid[x, y].tileType)) {
                debugConsole.PrintError($"Unit of type '{unitType}' cannot be spawned on tiles of type '{GameManager.Instance.tileGrid.grid[x, y].tileType}'!");
                return DebugConsole.DebugCommandCode.ParameterOutOfRange;
            }
            if (GameManager.Instance.tileGrid.grid[x, y].unitOnTile != null) {
                debugConsole.PrintError($"Tile at ({x}, {y}) already occupied!");
                return DebugConsole.DebugCommandCode.ParameterOutOfRange;
            }
            if (GameManager.Instance.tileGrid.grid[x, y].isCityTile) {
                if (!GameManager.Instance.GetCurrentPlayer().playerCities.Contains(GameManager.Instance.tileGrid.grid[x, y].gameObject.GetComponent<City>())) {
                    debugConsole.PrintError($"Player does not own city at ({x}, {y})!");
                    return DebugConsole.DebugCommandCode.ParameterOutOfRange;
                }
            }
            GameManager.Instance.GetCurrentPlayer().SpawnUnit(new Vector2Int(x, y), unitType);
            return DebugConsole.DebugCommandCode.Success;
        } else {
            return DebugConsole.DebugCommandCode.MissingParameters;
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
