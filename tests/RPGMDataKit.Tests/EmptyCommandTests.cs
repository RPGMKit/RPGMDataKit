using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.IfCommands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The empty command (code 0) that the editors put as the last command of every block, see <see cref="EmptyCommandHelper"/>.
/// The lists in the tests are written as the codes of the commands; the blocks are the editor's, 111 is If, 112 Loop, 102 Show Choices,
/// 301 Battle Processing, 230 is a command that opens nothing.
/// </summary>
public sealed class EmptyCommandTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    internal static NamedCommand Make(int code) => code switch
    {
        0 => new EmptyCommand(),
        111 => new IfSwitchCommand(),
        411 => new ElseCommand(),
        412 => new EndIfCommand(),
        112 => new LoopCommand(),
        413 => new EndLoopCommand(),
        102 => new ShowChoicesCommand(),
        402 => new ChoiceCaseCommand(),
        403 => new ChoiceCaseCancelCommand(),
        404 => new EndChoicesCommand(),
        301 => new BattleProcessingCommand(),
        601 => new BattleProcessingIfWinCommand(),
        602 => new BattleProcessingIfEscapeCommand(),
        603 => new BattleProcessingIfLoseCommand(),
        604 => new BattleProcessingEndCommand(),
        230 => new WaitCommand(),
        _ => throw new ArgumentException($"no command {code} in the test"),
    };

    internal static ICommonEvent EventOf(Version v, params int[] codes)
    {
        var commonEvent = v.New<ICommonEvent>();
        commonEvent.NamedCommands.Clear();
        foreach (var code in codes)
            commonEvent.NamedCommands.Add(Make(code));

        return commonEvent;
    }

    private static int[] Codes(ICommandContainer container) => container.NamedCommands.Select(c => c.Code).ToArray();

    /// <summary>The list as the editor makes it, with what the indent of the added commands must be.</summary>
    private static void Fixes(Version v, int[] without, int[] expected, int[]? expectedIndentsOfAdded = null)
    {
        var container = EventOf(v, without);

        Assert.Throws<EmptyCommandException>(() => EmptyCommandHelper.ValidateEmptyCommands(container));
        EmptyCommandHelper.SetupEmptyCommands(container);

        Assert.Equal(expected, Codes(container));
        EmptyCommandHelper.ValidateEmptyCommands(container);
        if (expectedIndentsOfAdded != null)
        {
            var added = container.NamedCommands.Where(c => c.Code == 0).Select(c => c.Indent!.Value).ToArray();
            Assert.Equal(expectedIndentsOfAdded, added);
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AnEmptyListIsOneEmptyCommand(Version v) => Fixes(v, [], [0], [0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheListEndsWithTheEmptyCommand(Version v) => Fixes(v, [230, 230], [230, 230, 0], [0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void EachBranchOfIfEndsWithIt(Version v) =>
        Fixes(v, [111, 230, 411, 230, 412], [111, 230, 0, 411, 230, 0, 412, 0], [1, 1, 0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void AnEmptyBranchIsJustTheEmptyCommand(Version v) =>
        Fixes(v, [111, 411, 412], [111, 0, 411, 0, 412, 0], [1, 1, 0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void LoopBodyEndsWithIt(Version v) => Fixes(v, [112, 230, 413], [112, 230, 0, 413, 0], [1, 0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void ChoicesHaveNoBlockBeforeTheFirstWhenButEveryWhenEndsWithIt(Version v) =>
        Fixes(v, [102, 402, 230, 402, 403, 230, 404], [102, 402, 230, 0, 402, 0, 403, 230, 0, 404, 0], [1, 1, 1, 0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void BattleProcessingBranchesEndWithIt(Version v) =>
        Fixes(v, [301, 601, 602, 230, 603, 604], [301, 601, 0, 602, 230, 0, 603, 0, 604, 0], [1, 1, 1, 0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void BattleProcessingWithoutBranchesOpensNothing(Version v) =>
        Fixes(v, [301, 230], [301, 230, 0], [0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void NestedBlocksGetTheirIndents(Version v) =>
        Fixes(v, [111, 102, 402, 112, 413, 404, 412],
            [111, 102, 402, 112, 0, 413, 0, 404, 0, 412, 0], [3, 2, 1, 0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheEmptyCommandsThatEndNoBlockAreRemoved(Version v) =>
        Fixes(v, [0, 230, 0, 0, 111, 0, 230, 0, 412], [230, 111, 230, 0, 412, 0]);

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheExceptionSaysWhereAnEmptyCommandIsMissing(Version v)
    {
        // the empty command is missing before Else (the index 2) and before Branch End (the index 4); the end of the list has it
        var error = Assert.Throws<EmptyCommandException>(() => EmptyCommandHelper.ValidateEmptyCommands(EventOf(v, 111, 230, 411, 230, 412, 0)));

        Assert.True(error.IsMissing);
        Assert.Equal(2, error.CommandIndex);
        Assert.Equal(411, error.CommandCode);
        Assert.Equal(1, error.MoreProblems);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheExceptionSaysWhereTheListEndsWithoutOne(Version v)
    {
        var error = Assert.Throws<EmptyCommandException>(() => EmptyCommandHelper.ValidateEmptyCommands(EventOf(v, 230, 230)));

        Assert.True(error.IsMissing);
        Assert.Equal(2, error.CommandIndex);
        Assert.Equal(0, error.CommandCode); // no command there
        Assert.Equal(0, error.MoreProblems);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheExceptionSaysWhichEmptyCommandIsNotNeeded(Version v)
    {
        var error = Assert.Throws<EmptyCommandException>(() => EmptyCommandHelper.ValidateEmptyCommands(EventOf(v, 230, 0, 230, 0)));

        Assert.False(error.IsMissing);
        Assert.Equal(1, error.CommandIndex);
        Assert.Equal(0, error.CommandCode);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AListThatIsRightIsLeftAlone(Version v)
    {
        var container = EventOf(v, 111, 0, 411, 0, 412, 102, 402, 0, 404, 0);

        EmptyCommandHelper.ValidateEmptyCommands(container);
        Assert.Equal(0, EmptyCommandHelper.SetupEmptyCommands(container));
        Assert.Equal([111, 0, 411, 0, 412, 102, 402, 0, 404, 0], Codes(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void RandomListsOfTheEditorAreRestoredFromWhatHasNoEmptyCommands(Version v)
    {
        var random = new Random(20261016);
        for (var round = 0; round < 300; ++round)
        {
            var editor = EditorList(random);

            var container = EventOf(v, editor.Where(c => c.Code != 0).Select(c => c.Code).ToArray());
            IndentHelper.SetupIndents(container, v.Name == "MV" ? StorageVersion.MV : StorageVersion.VXAce);
            EmptyCommandHelper.SetupEmptyCommands(container);

            Assert.Equal(editor, container.NamedCommands.Select(c => (c.Code, c.Indent!.Value)).ToList());
            EmptyCommandHelper.ValidateEmptyCommands(container);
        }
    }

    /// <summary>A random list as the editor makes it: the codes and the indents, the empty commands included.</summary>
    internal static List<(int Code, int Indent)> EditorList(Random random)
    {
        var list = new List<(int Code, int Indent)>();
        Block(random, list, 0, 0);
        list.Add((0, 0)); // the end of the list

        return list;
    }

    // the content of a block as the editor makes it, without the empty command that ends it
    private static void Block(Random random, List<(int, int)> list, int indent, int depth)
    {
        for (var n = random.Next(0, 4); n > 0; --n)
        {
            switch (depth < 4 ? random.Next(0, 6) : 0)
            {
                case 1: // If, with Else or without
                    list.Add((111, indent));
                    Branch(random, list, indent + 1, depth);
                    if (random.Next(2) == 0)
                    {
                        list.Add((411, indent));
                        Branch(random, list, indent + 1, depth);
                    }

                    list.Add((412, indent));
                    break;
                case 2:
                    list.Add((112, indent));
                    Branch(random, list, indent + 1, depth);
                    list.Add((413, indent));
                    break;
                case 3:
                    list.Add((102, indent));
                    var whens = random.Next(1, 4);
                    for (var i = 0; i < whens; ++i)
                    {
                        list.Add((402, indent));
                        Branch(random, list, indent + 1, depth);
                    }

                    if (random.Next(2) == 0)
                    {
                        list.Add((403, indent));
                        Branch(random, list, indent + 1, depth);
                    }

                    list.Add((404, indent));
                    break;
                case 4:
                    list.Add((301, indent));
                    var cases = new[] { 601, 602, 603 }.Where(_ => random.Next(2) == 0).ToList();
                    foreach (var branch in cases)
                    {
                        list.Add((branch, indent));
                        Branch(random, list, indent + 1, depth);
                    }

                    if (cases.Count > 0)
                        list.Add((604, indent));
                    else
                        list.Add((230, indent)); // without the empty commands, a Battle Processing without branches at the end of a branch cannot be told from the one before the next branch of the outer block
                    break;
                default:
                    list.Add((230, indent));
                    break;
            }
        }
    }

    private static void Branch(Random random, List<(int, int)> list, int indent, int depth)
    {
        Block(random, list, indent, depth + 1);
        list.Add((0, indent));
    }
}
