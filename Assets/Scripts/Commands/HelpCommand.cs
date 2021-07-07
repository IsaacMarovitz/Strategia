namespace Strategia {
    public class HelpCommand : IDebugCommand {
        public string commandId { get { return "help"; } }
        public string commandDescription { get { return "Shows list of available commands."; } }
        public string commandFormat { get { return "help"; } }

        public bool Process(string[] args, DebugConsole debugConsole) {
            debugConsole.PrintHelp();
            return true;
        }
    }
}