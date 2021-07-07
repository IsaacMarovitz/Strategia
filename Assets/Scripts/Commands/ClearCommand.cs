namespace Strategia {
    public class ClearCommand : IDebugCommand {
        public string commandId { get { return "clear"; } }
        public string commandDescription { get { return "Clears the console."; } }
        public string commandFormat { get { return "clear"; } }

        public bool Process(string[] args, DebugConsole debugConsole) {
            debugConsole.ClearConsole();
            return true;
        }
    }
}