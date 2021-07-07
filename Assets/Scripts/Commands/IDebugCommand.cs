namespace Strategia {
    public interface IDebugCommand {
        string commandId { get; }
        string commandDescription { get; }
        string commandFormat { get; }

        bool Process(string[] args, DebugConsole debugConsole);
    }

    public static class DebugUils {
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
}