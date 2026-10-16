namespace RPGMDataKit.Data.Structures.Exceptions;

/// <summary>
/// The commands of a list do not make a structure of blocks (see <c>IndentHelper.ValidateStructure</c>): a command is where it cannot be
/// or a block is not closed.
/// </summary>
public class CommandStructureException(string message, int commandIndex, int commandCode)
    : CommandListException(message, commandIndex, commandCode);
