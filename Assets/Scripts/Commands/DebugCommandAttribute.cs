using System;

[AttributeUsage(AttributeTargets.Method)]
public class DebugCommandAttribute : Attribute {
    public string commandId { get; }
    public string commandDescription { get; }
    public string commandFormat { get; }

    public DebugCommandAttribute(string commandId, string commandDescription, string commandFormat) {
        this.commandId = commandId;
        this.commandDescription = commandDescription;
        this.commandFormat = commandFormat;
    }
}
