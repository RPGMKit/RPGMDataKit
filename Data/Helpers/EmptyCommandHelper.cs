using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// Where the editors put the empty command (code 0, the "@>" line where a new command is inserted): the last command of
/// every block, and nowhere else. The blocks are
/// <list type="bullet">
/// <item>the whole list of commands;</item>
/// <item>the branch of <c>If</c> (up to <c>Else</c> or <c>Branch End</c>) and the <c>Else</c> branch;</item>
/// <item>the body of <c>Loop</c>;</item>
/// <item>every <c>When</c> branch and the <c>When Cancel</c> branch of <c>Show Choices</c>;</item>
/// <item>every <c>If Win</c>, <c>If Escape</c> and <c>If Lose</c> branch of <c>Battle Processing</c>.</item>
/// </list>
/// A list that has no commands is a single empty command. There is no block (so no empty command) between <c>Show Choices</c>
/// and its first <c>When</c>, between <c>Battle Processing</c> and its first branch, and in a <c>Battle Processing</c> that has no
/// branches at all. This is what all of 440 thousand commands of a real game (three versions of it) do, with no exception.
/// The engines ignore the empty command, the editors expect it.
/// </summary>
public static class EmptyCommandHelper
{
    private static readonly int[] Branches =
    [
        CommandCodes.Else, CommandCodes.ChoiceCase, CommandCodes.ChoiceCaseCancel,
        CommandCodes.BattleProcessingIfWin, CommandCodes.BattleProcessingIfEscape, CommandCodes.BattleProcessingIfLose,
    ];

    private static readonly int[] Ends =
        [CommandCodes.EndIf, CommandCodes.ChoicesEnd, CommandCodes.EndLoop, CommandCodes.BattleProcessingEnd];

    /// <summary>What is wrong with the empty commands at a place of the list.</summary>
    private enum ProblemKind
    {
        /// <summary>An empty command that ends a block is not there; it has to be inserted.</summary>
        Missing,

        /// <summary>An empty command that ends no block is there; it has to be removed.</summary>
        Extra,
    }

    /// <summary>A place where the empty commands are not as the editor puts them: the index, the code of the command there, what is wrong, the indent of the command to insert, the text.</summary>
    private readonly record struct Problem(int Index, int Code, ProblemKind Kind, int Indent, string Reason);

    /// <summary>
    /// Throws an <see cref="EmptyCommandException"/> (with the index of the first problem) if the empty commands of the container are not
    /// where the editor puts them.
    /// </summary>
    public static void ValidateEmptyCommands(ICommandContainer container)
    {
        var problems = FindProblems(container);
        if (problems.Count == 0)
            return;

        var first = problems[0];
        throw new EmptyCommandException(problems.Count == 1 ? first.Reason : $"{first.Reason} (and {problems.Count - 1} more)",
            first.Index, first.Code, first.Kind == ProblemKind.Missing, problems.Count - 1);
    }

    /// <summary>
    /// Puts the empty commands where the editor puts them: adds the missing ones (with the indent of the block) and removes the
    /// ones that end no block, which the engines ignore. The other commands keep their indents; to set them, call
    /// <see cref="IndentHelper.SetupIndents"/> first (the structure of the commands is what counts here, not their indents).
    /// </summary>
    /// <returns>The number of the commands added and removed.</returns>
    public static int SetupEmptyCommands(ICommandContainer container)
    {
        var problems = FindProblems(container);

        // from the end, so that the indexes of the problems that are not done yet stay right
        for (var i = problems.Count - 1; i >= 0; --i)
        {
            var problem = problems[i];
            if (problem.Kind == ProblemKind.Extra)
                container.NamedCommands.RemoveAt(problem.Index);
            else
                container.NamedCommands.Insert(problem.Index, new EmptyCommand { Indent = problem.Indent });
        }

        return problems.Count;
    }

    private static List<Problem> FindProblems(ICommandContainer container)
    {
        if (container is IMoveRoute)
            throw new ArgumentException("The rules are for the commands of events; a move route has its own commands.", nameof(container));

        var commands = container.NamedCommands;
        var problems = new List<Problem>();

        var level = 0; // the indent of the content of the block that the position is in
        var inBlock = true; // the position is in a block that has to end with an empty command; the list itself is one
        var branchesMayFollow = false; // after Show Choices and Battle Processing, up to the next command

        for (var i = 0; i < commands.Count; ++i)
        {
            var code = commands[i].Code;

            if (branchesMayFollow)
            {
                branchesMayFollow = false;
                if (code is CommandCodes.ChoiceCase or CommandCodes.ChoiceCaseCancel
                    or CommandCodes.BattleProcessingIfWin or CommandCodes.BattleProcessingIfEscape
                    or CommandCodes.BattleProcessingIfLose)
                {
                    ++level;
                }
                else
                {
                    inBlock = true; // no branches: the commands go on in the block that Battle Processing is in
                }
            }

            if (code == CommandCodes.Empty)
            {
                if (i + 1 < commands.Count && !IsBranchOrEnd(commands[i + 1].Code))
                    problems.Add(new Problem(i, CommandCodes.Empty, ProblemKind.Extra, 0, $"Empty command at index {i} does not end a block"));

                continue;
            }

            if (IsBranchOrEnd(code))
            {
                if (inBlock && (i == 0 || commands[i - 1].Code != CommandCodes.Empty))
                    problems.Add(new Problem(i, code, ProblemKind.Missing, level, $"No empty command before command {code} at index {i}"));

                inBlock = true;
                if (Array.IndexOf(Ends, code) >= 0)
                    level = Math.Max(0, level - 1);

                continue;
            }

            switch (code)
            {
                case CommandCodes.If:
                case CommandCodes.Loop:
                    ++level;
                    break;
                case CommandCodes.ShowChoices:
                case CommandCodes.BattleProcessing:
                    inBlock = false;
                    branchesMayFollow = true;
                    break;
            }
        }

        if (branchesMayFollow)
            inBlock = true;

        if (inBlock && (commands.Count == 0 || commands[^1].Code != CommandCodes.Empty))
            problems.Add(new Problem(commands.Count, CommandCodes.Empty, ProblemKind.Missing, level, "No empty command at the end of the list"));

        return problems;
    }

    internal static bool IsBranchOrEnd(int code) => Array.IndexOf(Branches, code) >= 0 || Array.IndexOf(Ends, code) >= 0;
}
