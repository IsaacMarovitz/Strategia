namespace Strategia {
    public class TestWithNumCommand : IDebugCommand {
        public string commandId { get { return "test_with_num"; } }
        public string commandDescription { get { return "Test the debug console with num."; } }
        public string commandFormat { get { return "test_with_num <num>"; } }

        public bool Process(string[] args, DebugConsole debugConsole) {
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
    }
}