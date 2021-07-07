namespace Strategia {
    public class ClearFogCommand : IDebugCommand {
        public string commandId { get { return "clear_fog"; } }
        public string commandDescription { get { return "Clears the fog of war for the current player."; } }
        public string commandFormat { get { return "clear_fog <bool>"; } }

        public bool Process(string[] args, DebugConsole debugConsole) {
            if (args.Length > 0) {
                if (DebugUils.ParseBool(args[0], out bool boolValue)) {
                    GameManager.Instance.GetCurrentPlayer().RevealAllTiles(boolValue);
                    if (boolValue) {
                        debugConsole.PrintSuccess($"Fog of War for Player {GameManager.Instance.currentPlayerIndex} cleared.");
                    } else {
                        debugConsole.PrintSuccess($"Re-enabled Fog of War for Player {GameManager.Instance.currentPlayerIndex}.");
                    }
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