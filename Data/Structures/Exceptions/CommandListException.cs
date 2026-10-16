namespace RPGMDataKit.Data.Structures.Exceptions;

/// <summary>A list of commands is not as the editors make it; says which command is the first one that is wrong.</summary>
public abstract class CommandListException : Exception
{
    /// <param name="message">What is wrong.</param>
    /// <param name="commandIndex">The index of the command in the list.</param>
    /// <param name="commandCode">The code of that command.</param>
    protected CommandListException(string message, int commandIndex, int commandCode) : base(message)
    {
        CommandIndex = commandIndex;
        CommandCode = commandCode;
    }

    /// <summary>The index of the command in the list: the one that is out of place, or the one that opens a block that is never closed.</summary>
    public int CommandIndex { get; }

    /// <summary>The code of that command.</summary>
    public int CommandCode { get; }
}
