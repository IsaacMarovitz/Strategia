namespace Strategia {
    public class FastProdCommand : IDebugCommand {
        public string commandId { get { return "fast_prod"; } }
        public string commandDescription { get { return "Changes all unit TTCs to 1 day."; } }
        public string commandFormat { get { return "fast_prod <bool>"; } }

        public bool Process(string[] args, DebugConsole debugConsole) {
            if (args.Length > 0) {
                if (DebugUils.ParseBool(args[0], out bool boolValue)) {
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
    }
}