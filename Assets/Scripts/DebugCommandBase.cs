using System;

public class DebugCommandBase {
    private string _commandId;
    private string _commandDescription;
    private string _commandFormat;
    private bool _printCommandCompleteMsg;

    public string commandId { get { return _commandId; } }
    public string commandDescription { get { return _commandDescription; } }
    public string commmandFormat { get { return _commandFormat; } }
    public bool printCommandCompleteMsg { get { return _printCommandCompleteMsg; } }

    public DebugCommandBase(string id, string description, string format, bool printCommandCompleteMsg) {
        _commandId = id;
        _commandDescription = description;
        _commandFormat = format;
        _printCommandCompleteMsg = printCommandCompleteMsg;
    }
}

public class DebugCommand : DebugCommandBase {
    private Action command;

    public DebugCommand(string id, string description, string format, bool printCommandCompleteMsg, Action command) : base (id, description, format, printCommandCompleteMsg) {
        this.command = command;
    }

    public void Invoke() {
        command.Invoke();
    }
}

public class DebugCommand<T> : DebugCommandBase {
    private Action<T> command;

    public DebugCommand(string id, string description, string format, bool printCommandCompleteMsg, Action<T> command) : base (id, description, format, printCommandCompleteMsg) {
        this.command = command;
    }

    public void Invoke(T value) {
        command.Invoke(value);
    }
}