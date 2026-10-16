using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers;

public static class IndentHelper
{
    private delegate void IndentWalker(int currIndent, int commandIndex, NamedCommand? prevCommand, NamedCommand currCommand);
    
    /// <summary>Throws a <see cref="CommandIndentException"/> (with the index of the command) if a command has not the indent that its place in the blocks gives it.</summary>
    public static void ValidateIndents(ICommandContainer container, StorageVersion version)
    {
        WalkForIndents(container, version, (indent, index, prevCommand, currCommand) =>
        {
            if (indent != currCommand.Indent)
            {
                throw new CommandIndentException(
                    $"Wrong indent '{currCommand.Indent}' for command {currCommand.Code} at index {index}, expected {indent}, prevCommand: {prevCommand?.Code}",
                    index, currCommand.Code, indent, currCommand.Indent);
            }
        });
    }

    /// <summary>
    /// Throws a <see cref="CommandStructureException"/> (with the index and the code of the command) if the commands that open and close blocks do not make a structure: an <c>Else</c>, a <c>Branch End</c> or a branch
    /// without the command that opens it, a block that is never closed, two <c>Else</c> in an <c>If</c>, a <c>When</c> after
    /// <c>When Cancel</c>, branches of <c>Battle Processing</c> not in the order of Win, Escape, Lose, a command of
    /// <c>Show Choices</c> or <c>Battle Processing</c> that is not in a branch, a <c>Show Choices</c> without a <c>When</c>.
    /// The editors never make such a list and the engines do not check it: the indents of the commands then give a different
    /// walk than the codes do. It is not a part of <see cref="ValidateIndents"/> because that one compares the indents only.
    /// </summary>
    public static void ValidateStructure(ICommandContainer container)
    {
        var commands = container.NamedCommands;
        var open = new Stack<Block>();

        for (var i = 0; i < commands.Count; ++i)
        {
            var code = commands[i].Code;
            var top = open.Count > 0 ? open.Peek() : null;

            if (top is { Opener: CommandCodes.ShowChoices or CommandCodes.BattleProcessing, LastBranch: 0 } && !IsBranchOf(top.Opener, code)
                && code != CommandCodes.ChoicesEnd && code != CommandCodes.BattleProcessingEnd)
            {
                throw Wrong(i, code, $"is not in a branch of command {top.Opener}");
            }

            switch (code)
            {
                case CommandCodes.If:
                case CommandCodes.Loop:
                case CommandCodes.ShowChoices:
                    open.Push(new Block(code, i));
                    break;
                case CommandCodes.BattleProcessing:
                    // without branches it opens nothing, same as in the indents
                    if (i + 1 < commands.Count && IsBranchOf(CommandCodes.BattleProcessing, commands[i + 1].Code))
                        open.Push(new Block(code, i));
                    break;
                case CommandCodes.Else:
                    Require(top, CommandCodes.If, i, code);
                    if (top!.LastBranch != 0)
                        throw Wrong(i, code, "is the second Else of an If");
                    top.LastBranch = code;
                    break;
                case CommandCodes.EndIf:
                    Require(top, CommandCodes.If, i, code);
                    open.Pop();
                    break;
                case CommandCodes.EndLoop:
                    Require(top, CommandCodes.Loop, i, code);
                    open.Pop();
                    break;
                case CommandCodes.ChoiceCase:
                case CommandCodes.ChoiceCaseCancel:
                    Require(top, CommandCodes.ShowChoices, i, code);
                    if (top!.LastBranch == CommandCodes.ChoiceCaseCancel)
                        throw Wrong(i, code, "comes after When Cancel");
                    top.LastBranch = code;
                    break;
                case CommandCodes.ChoicesEnd:
                    Require(top, CommandCodes.ShowChoices, i, code);
                    if (top!.LastBranch == 0)
                        throw Wrong(i, code, "closes Show Choices that has no When");
                    open.Pop();
                    break;
                case CommandCodes.BattleProcessingIfWin:
                case CommandCodes.BattleProcessingIfEscape:
                case CommandCodes.BattleProcessingIfLose:
                    Require(top, CommandCodes.BattleProcessing, i, code);
                    if (code <= top!.LastBranch)
                        throw Wrong(i, code, $"comes after {top.LastBranch}, the branches go in the order Win, Escape, Lose");
                    top.LastBranch = code;
                    break;
                case CommandCodes.BattleProcessingEnd:
                    Require(top, CommandCodes.BattleProcessing, i, code);
                    open.Pop();
                    break;
            }
        }

        if (open.Count > 0)
        {
            var unclosed = open.Peek();
            throw new CommandStructureException($"Command {unclosed.Opener} at index {unclosed.Index} is never closed", unclosed.Index, unclosed.Opener);
        }
    }

    private sealed class Block(int opener, int index)
    {
        public int Opener { get; } = opener;

        /// <summary>The index of the command that opens the block.</summary>
        public int Index { get; } = index;

        /// <summary>The code of the last branch (Else, When, When Cancel, If Win...), 0 before the first one.</summary>
        public int LastBranch { get; set; }
    }

    private static bool IsBranchOf(int opener, int code) => opener switch
    {
        CommandCodes.ShowChoices => code is CommandCodes.ChoiceCase or CommandCodes.ChoiceCaseCancel,
        CommandCodes.BattleProcessing => code is CommandCodes.BattleProcessingIfWin or CommandCodes.BattleProcessingIfEscape
            or CommandCodes.BattleProcessingIfLose,
        _ => false,
    };

    private static void Require(Block? top, int opener, int index, int code)
    {
        if (top == null || top.Opener != opener)
            throw Wrong(index, code, $"has no command {opener} that it belongs to" + (top == null ? "" : $" (the open block is {top.Opener})"));
    }

    private static CommandStructureException Wrong(int index, int code, string what) => new($"Command {code} at index {index} {what}", index, code);

    public static void SetupIndents(ICommandContainer container, StorageVersion version)
    {
        WalkForIndents(container, version, (indent, _, _, command) => { command.Indent = indent; });
    }

    private static void WalkForIndents(ICommandContainer container, StorageVersion version, IndentWalker walker)
    {
        NamedCommand? prevCommand = null;

        var commands = container.NamedCommands.ToList();

        int i = 0, indentLevel = 0;
        foreach (var c in commands)
        {
            switch (c.Code)
            {
                case CommandCodes.EndIf:
                    --indentLevel;
                    break;
                case CommandCodes.Else:
                    --indentLevel;
                    break;
                case CommandCodes.EndLoop:
                    --indentLevel;
                    break;
                case CommandCodes.ChoiceCase:
                case CommandCodes.ChoiceCaseCancel:
                    --indentLevel;
                    break;
                case CommandCodes.BattleProcessingIfLose:
                case CommandCodes.BattleProcessingIfEscape:
                case CommandCodes.BattleProcessingIfWin:
                    --indentLevel;
                    break;
                case CommandCodes.BattleProcessingEnd:
                    --indentLevel;
                    break;
                case CommandCodes.ChoicesEnd:
                    --indentLevel;
                    break;
            }

            walker(indentLevel, i, prevCommand, c);

            prevCommand = c;

            switch (c.Code)
            {
                case CommandCodes.If:
                    ++indentLevel;
                    break;
                case CommandCodes.Loop:
                    ++indentLevel;
                    break;
                case CommandCodes.Else:
                    ++indentLevel;
                    break;
                case CommandCodes.ChoiceCase:
                case CommandCodes.ChoiceCaseCancel:
                    ++indentLevel;
                    break;
                case CommandCodes.BattleProcessingIfLose:
                case CommandCodes.BattleProcessingIfEscape:
                case CommandCodes.BattleProcessingIfWin:
                    ++indentLevel;
                    break;
                case CommandCodes.BattleProcessing:
                    var nextCommand = commands.ElementAtOrDefault(i + 1);
                    if (nextCommand != null && (nextCommand.Code == CommandCodes.BattleProcessingIfLose
                                                || nextCommand.Code == CommandCodes.BattleProcessingIfEscape
                                                || nextCommand.Code == CommandCodes.BattleProcessingIfWin))
                    {
                        ++indentLevel;
                    }

                    break;
                case CommandCodes.ShowChoices:
                    ++indentLevel;
                    break;
            }

            ++i;
        }
    }
}