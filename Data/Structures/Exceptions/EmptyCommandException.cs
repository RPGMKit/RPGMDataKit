namespace RPGMDataKit.Data.Structures.Exceptions;

/// <summary>
/// An empty command (code 0) is not where the editors put it (see <c>EmptyCommandHelper.ValidateEmptyCommands</c>): one that ends a block is missing,
/// or one that ends no block is there.
/// </summary>
public class EmptyCommandException : CommandListException
{
    /// <param name="message">What is wrong.</param>
    /// <param name="commandIndex">The index where the empty command is missing, or the index of the one that is not needed.</param>
    /// <param name="commandCode">The code of the command at that index: the one that the empty command should stand before (0 at the end of the list, where there is none); 0 for the empty command that is not needed.</param>
    /// <param name="isMissing">True if an empty command has to be inserted at the index, false if the command at the index has to be removed.</param>
    /// <param name="moreProblems">The number of other problems in the list.</param>
    public EmptyCommandException(string message, int commandIndex, int commandCode, bool isMissing, int moreProblems)
        : base(message, commandIndex, commandCode)
    {
        IsMissing = isMissing;
        MoreProblems = moreProblems;
    }

    /// <summary>True if an empty command has to be inserted at <see cref="CommandListException.CommandIndex"/>, false if the one there has to be removed.</summary>
    public bool IsMissing { get; }

    /// <summary>The number of problems after this one (this is the first); <see cref="EmptyCommandHelper.SetupEmptyCommands"/> fixes them all.</summary>
    public int MoreProblems { get; }
}
