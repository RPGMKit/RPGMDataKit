using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// Changes the commands of a container: puts the commands of a <see cref="CommandBuilder"/> (or commands that are made by hand) at the end, into a
/// place, instead of a command or of all of them, and removes a command. Afterwards the empty commands, the lines of the movement routes and the
/// indents of the whole list are set (<see cref="EmptyCommandHelper.SetupEmptyCommands"/>, <see cref="RouteLinesHelper.SetupRouteLines"/>,
/// <see cref="IndentHelper.SetupIndents(ICommandContainer)"/>), so the list is as the editor makes it also where it was not before.
/// <para>
/// The places are the ones of the editors. A command is put in place of the one with the index, which goes down; the end of a block is its empty
/// command (give its index to put the commands at the end of the block). The lines of a command of several lines and what divides or ends a block
/// (<c>When</c>, <c>Else</c>, <c>Branch End</c>...) are not places: <see cref="CommandPlaceException"/>. A command that is replaced or removed
/// goes with all it holds: a text with its lines, an <c>If</c> with its branches up to <c>Branch End</c>, a movement route with its lines.
/// </para>
/// </summary>
public static class CommandContainerExtensions
{
    // The lines of a command of several lines, and what divides or ends a block: not a place for a command.
    private static readonly HashSet<int> NotAPlace =
    [
        CommandCodes.ShowTextLine, CommandCodes.ShowScrollingTextLine, CommandCodes.CommentContinue, CommandCodes.ScriptLine,
        CommandCodes.ShopProcessingLine, CommandCodes.MovementRouteCommand,
        CommandCodes.ChoiceCase, CommandCodes.ChoiceCaseCancel, CommandCodes.ChoicesEnd,
        CommandCodes.Else, CommandCodes.EndIf, CommandCodes.EndLoop,
        CommandCodes.BattleProcessingIfWin, CommandCodes.BattleProcessingIfEscape, CommandCodes.BattleProcessingIfLose, CommandCodes.BattleProcessingEnd,
    ];

    private static readonly HashSet<int> Lines =
    [
        CommandCodes.ShowTextLine, CommandCodes.ShowScrollingTextLine, CommandCodes.CommentContinue, CommandCodes.ScriptLine,
        CommandCodes.ShopProcessingLine, CommandCodes.MovementRouteCommand,
    ];

    // a command of several lines: the code of its lines
    private static readonly Dictionary<int, int> LinesOf = new()
    {
        [CommandCodes.ShowTextBase] = CommandCodes.ShowTextLine,
        [CommandCodes.ShowScrollingTextBase] = CommandCodes.ShowScrollingTextLine,
        [CommandCodes.Comment] = CommandCodes.CommentContinue,
        [CommandCodes.Script] = CommandCodes.ScriptLine,
        [CommandCodes.ShopProcessing] = CommandCodes.ShopProcessingLine,
        [CommandCodes.SetMovementRoute] = CommandCodes.MovementRouteCommand,
    };

    // ---- Append ----

    /// <summary>Puts the commands at the end of the list, before the empty command that ends it, if there is one.</summary>
    public static void Append(this ICommandContainer container, Action<CommandBuilder> build) =>
        Put(container, EndOfList(container), Make(container, build));

    /// <summary><see cref="Append(ICommandContainer, Action{CommandBuilder})"/> for commands that are made by hand (the lines of a command of several lines too).</summary>
    public static void Append(this ICommandContainer container, params NamedCommand[] commands) =>
        Put(container, EndOfList(container), Checked(commands));

    // ---- InsertAt ----

    /// <summary>
    /// Puts the commands in place of the command with the index, as the editors do (the old command and the ones after it go down). To put them at
    /// the end of a block, give the index of the empty command that ends it. The length of the list is the end of it (the same as
    /// <see cref="Append(ICommandContainer, Action{CommandBuilder})"/>).
    /// </summary>
    /// <exception cref="CommandPlaceException">The command with the index is a line of a command of several lines, or what divides or ends a block.</exception>
    public static void InsertAt(this ICommandContainer container, int index, Action<CommandBuilder> build)
    {
        index = InsertionIndex(container, index);
        Put(container, index, Make(container, build));
    }

    /// <summary><see cref="InsertAt(ICommandContainer, int, Action{CommandBuilder})"/> for commands that are made by hand.</summary>
    public static void InsertAt(this ICommandContainer container, int index, params NamedCommand[] commands)
    {
        index = InsertionIndex(container, index);
        Put(container, index, Checked(commands));
    }

    // ---- Replace ----

    /// <summary>Replaces all commands of the list with the commands.</summary>
    public static void Replace(this ICommandContainer container, Action<CommandBuilder> build)
    {
        var made = Make(container, build);

        container.NamedCommands.Clear();
        Put(container, 0, made);
    }

    /// <summary><see cref="Replace(ICommandContainer, Action{CommandBuilder})"/> for commands that are made by hand.</summary>
    public static void Replace(this ICommandContainer container, params NamedCommand[] commands)
    {
        var made = Checked(commands);

        container.NamedCommands.Clear();
        Put(container, 0, made);
    }

    // ---- ReplaceAt ----

    /// <summary>
    /// Replaces the command with the index, and all it holds (a text with its lines, an <c>If</c> with its branches up to <c>Branch End</c>, a movement
    /// route with its lines), with the commands.
    /// </summary>
    /// <exception cref="CommandPlaceException">
    /// The command is a line of a command of several lines, what divides or ends a block, or the empty command that ends a block or the list.
    /// </exception>
    public static void ReplaceAt(this ICommandContainer container, int index, Action<CommandBuilder> build)
    {
        var end = RangeEnd(container, index, "replace");
        var made = Make(container, build);

        RemoveRange(container, index, end);
        Put(container, index, made);
    }

    /// <summary><see cref="ReplaceAt(ICommandContainer, int, Action{CommandBuilder})"/> for commands that are made by hand.</summary>
    public static void ReplaceAt(this ICommandContainer container, int index, params NamedCommand[] commands)
    {
        var end = RangeEnd(container, index, "replace");
        var made = Checked(commands);

        RemoveRange(container, index, end);
        Put(container, index, made);
    }

    // ---- RemoveAt ----

    /// <summary>Removes the command with the index, and all it holds (see <see cref="ReplaceAt(ICommandContainer, int, Action{CommandBuilder})"/>).</summary>
    /// <exception cref="CommandPlaceException">The same as for <c>ReplaceAt</c>.</exception>
    public static void RemoveAt(this ICommandContainer container, int index)
    {
        var end = RangeEnd(container, index, "remove");

        RemoveRange(container, index, end);
        Put(container, index, []);
    }

    // ---- where ----

    // the end of the list is before the empty command that ends it
    private static int EndOfList(ICommandContainer container)
    {
        var commands = container.NamedCommands;

        return commands.Count > 0 && commands[^1].Code == CommandCodes.Empty ? commands.Count - 1 : commands.Count;
    }

    private static int InsertionIndex(ICommandContainer container, int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, container.NamedCommands.Count);

        if (index == container.NamedCommands.Count)
            return EndOfList(container);

        EnsureAPlace(container, index, "insert");

        return index;
    }

    /// <summary>The index after the command with the index and all it holds. The command has to be one that can be replaced or removed.</summary>
    private static int RangeEnd(ICommandContainer container, int index, string operation)
    {
        var commands = container.NamedCommands;
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, commands.Count);

        EnsureAPlace(container, index, operation);
        var code = commands[index].Code;
        if (code == CommandCodes.Empty)
        {
            throw new CommandPlaceException(
                $"Cannot {operation} at the index {index}: it is the empty command that ends a block or the list, which the editors do not let go. "
                + $"To put commands at the end of the block use InsertAt({index}, ...).", index, code);
        }

        if (LinesOf.TryGetValue(code, out var lineCode))
        {
            var end = index + 1;
            while (end < commands.Count && commands[end].Code == lineCode)
                ++end;

            return end;
        }

        if (!IsBlockHead(commands, index))
            return index + 1;

        var depth = 0;
        for (var i = index; i < commands.Count; i++)
        {
            if (IsBlockHead(commands, i))
            {
                ++depth;
            }
            else if (IsBlockEnd(commands[i].Code) && --depth == 0)
            {
                return i + 1;
            }
        }

        throw new CommandPlaceException($"Cannot {operation} at the index {index}: the block that it opens is not closed.", index, code);
    }

    private static bool IsBlockHead(IList<NamedCommand> commands, int i) => commands[i].Code switch
    {
        CommandCodes.If or CommandCodes.Loop or CommandCodes.ShowChoices => true,
        // without branches Battle Processing is one command, as it is for the indents
        CommandCodes.BattleProcessing => i + 1 < commands.Count
            && commands[i + 1].Code is CommandCodes.BattleProcessingIfWin or CommandCodes.BattleProcessingIfEscape or CommandCodes.BattleProcessingIfLose,
        _ => false,
    };

    private static bool IsBlockEnd(int code) =>
        code is CommandCodes.EndIf or CommandCodes.EndLoop or CommandCodes.ChoicesEnd or CommandCodes.BattleProcessingEnd;

    private static void EnsureAPlace(ICommandContainer container, int index, string operation)
    {
        var commands = container.NamedCommands;
        var code = commands[index].Code;
        if (!NotAPlace.Contains(code))
            return;

        var change = operation == "insert";
        if (Lines.Contains(code))
        {
            var head = index;
            while (head > 0 && Lines.Contains(commands[head].Code))
                --head;

            var after = index;
            while (after < commands.Count && Lines.Contains(commands[after].Code))
                ++after;

            throw new CommandPlaceException(
                change
                    ? $"Commands cannot be put at the index {index}: it is a line of the command at the index {head}. Use {head} (before the command) or {after} (after its lines)."
                    : $"Cannot {operation} at the index {index}: it is a line of the command at the index {head}. Use {head}: the command goes with its lines.",
                index, code);
        }

        throw new CommandPlaceException(
            change
                ? $"Commands cannot be put at the index {index}: it divides or ends a block. Use {index - 1} (the empty command that ends the block above: the commands go to its end) or {index + 1} (after it)."
                : $"Cannot {operation} at the index {index}: it divides or ends a block. Use the command that opens the block: it goes with everything in the block.",
            index, code);
    }

    // ---- doing ----

    private static List<NamedCommand> Checked(NamedCommand[] commands)
    {
        ArgumentNullException.ThrowIfNull(commands);
        foreach (var command in commands)
            ArgumentNullException.ThrowIfNull(command);

        return [.. commands];
    }

    // the builder is for the maker of the container: the objects in the commands (a route, a sound) are of the classes of that maker
    private static List<NamedCommand> Make(ICommandContainer container, Action<CommandBuilder> build)
    {
        if (container.NamedCommands is not ICommandListTypes list)
            throw new NotSupportedException("The commands of this container are not a list of the library, so its maker is not known.");

        var builder = new CommandBuilder(list.Types);
        build(builder);

        return builder.Commands.ToList();
    }

    private static void RemoveRange(ICommandContainer container, int index, int end)
    {
        for (var i = end - 1; i >= index; i--)
            container.NamedCommands.RemoveAt(i);
    }

    private static void Put(ICommandContainer container, int position, List<NamedCommand> commands)
    {
        foreach (var command in commands)
            container.NamedCommands.Insert(position++, command);

        EmptyCommandHelper.SetupEmptyCommands(container);
        RouteLinesHelper.SetupRouteLines(container);
        IndentHelper.SetupIndents(container);
    }
}
