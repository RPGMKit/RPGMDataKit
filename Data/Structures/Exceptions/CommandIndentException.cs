namespace RPGMDataKit.Data.Structures.Exceptions;

/// <summary>A command has not the indent that its place in the blocks gives it (see <c>IndentHelper.ValidateIndents</c>).</summary>
public class CommandIndentException : CommandListException
{
    public CommandIndentException(string message, int commandIndex, int commandCode, int expectedIndent, int? actualIndent)
        : base(message, commandIndex, commandCode)
    {
        ExpectedIndent = expectedIndent;
        ActualIndent = actualIndent;
    }

    /// <summary>The indent that the structure of the blocks gives the command.</summary>
    public int ExpectedIndent { get; }

    /// <summary>The indent that the command has; null if it has none.</summary>
    public int? ActualIndent { get; }
}
