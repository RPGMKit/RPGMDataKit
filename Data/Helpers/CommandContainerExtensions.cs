using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// Puts the commands of a <see cref="CommandBuilder"/> into a container. Afterwards the empty commands and the indents of the whole list are
/// set (<see cref="EmptyCommandHelper.SetupEmptyCommands"/>, <see cref="IndentHelper.SetupIndents(ICommandContainer)"/>), so the list is as the
/// editor makes it also where it was not before.
/// </summary>
public static class CommandContainerExtensions
{
    /// <summary>Puts the commands at the end of the list, before the empty command that ends it, if there is one.</summary>
    public static void AppendCommands(this ICommandContainer container, Action<CommandBuilder> build)
    {
        Splice(container, container.NamedCommands.Count, build);
    }

    /// <summary>
    /// Puts the commands before the command with the index (the length of the list is the end of it). Where the index is the end of a block, the
    /// commands go to the end of that block, before the empty command that ends it.
    /// </summary>
    public static void InsertCommands(this ICommandContainer container, int index, Action<CommandBuilder> build)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, container.NamedCommands.Count);

        Splice(container, index, build);
    }

    /// <summary>Replaces all commands of the list with the commands.</summary>
    public static void ReplaceCommands(this ICommandContainer container, Action<CommandBuilder> build)
    {
        var made = Make(build);

        container.NamedCommands.Clear();
        Put(container, 0, made);
    }

    private static void Splice(ICommandContainer container, int position, Action<CommandBuilder> build)
    {
        var made = Make(build);

        // the empty command in front of the end of a block (or of the list) ends the block: the new commands go before it, into the block
        var commands = container.NamedCommands;
        if (position > 0 && commands[position - 1].Code == Enums.CommandCodes.Empty
            && (position == commands.Count || EmptyCommandHelper.IsBranchOrEnd(commands[position].Code)))
        {
            --position;
        }

        Put(container, position, made);
    }

    private static List<NamedCommand> Make(Action<CommandBuilder> build)
    {
        var builder = new CommandBuilder();
        build(builder);

        return builder.Commands.ToList();
    }

    private static void Put(ICommandContainer container, int position, List<NamedCommand> commands)
    {
        foreach (var command in commands)
            container.NamedCommands.Insert(position++, command);

        EmptyCommandHelper.SetupEmptyCommands(container);
        IndentHelper.SetupIndents(container);
    }
}
