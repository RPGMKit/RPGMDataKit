using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Exceptions;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// <see cref="IndentHelper.ValidateIndents"/> and <see cref="IndentHelper.ValidateStructure"/> report the first command that is wrong in a typed
/// exception. About the structure: the commands that open and close blocks must make a structure. Lists are written as the
/// codes of the commands (see <see cref="EmptyCommandTests"/>).
/// </summary>
public sealed class CommandStructureTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    public static IEnumerable<object[]> Right() =>
        Version.All.SelectMany(v => new[]
        {
            new object[] { v, Array.Empty<int>() },
            new object[] { v, new[] { 230 } },
            new object[] { v, new[] { 111, 412 } },
            new object[] { v, new[] { 111, 230, 411, 230, 412, 0 } },
            new object[] { v, new[] { 112, 413 } },
            new object[] { v, new[] { 102, 402, 402, 403, 404 } },
            new object[] { v, new[] { 102, 402, 230, 0, 404 } },
            new object[] { v, new[] { 301 } },
            new object[] { v, new[] { 301, 230 } },
            new object[] { v, new[] { 301, 601, 604 } },
            new object[] { v, new[] { 301, 601, 602, 603, 604 } },
            new object[] { v, new[] { 301, 602, 604 } },
            new object[] { v, new[] { 111, 112, 102, 402, 301, 601, 602, 604, 404, 413, 411, 412 } },
        });

    public static IEnumerable<object[]> Wrong() =>
        Version.All.SelectMany(v => new[]
        {
            new object[] { v, new[] { 411 } },                                // Else without If
            new object[] { v, new[] { 412 } },                                // Branch End without If
            new object[] { v, new[] { 413 } },                                // Repeat Above without Loop
            new object[] { v, new[] { 402 } },                                // When without Show Choices
            new object[] { v, new[] { 404 } },
            new object[] { v, new[] { 601 } },                                // a branch without Battle Processing
            new object[] { v, new[] { 604 } },
            new object[] { v, new[] { 111 } },                                // never closed
            new object[] { v, new[] { 112, 111, 412 } },
            new object[] { v, new[] { 102, 402 } },
            new object[] { v, new[] { 301, 601 } },
            new object[] { v, new[] { 111, 411, 411, 412 } },                 // two Else
            new object[] { v, new[] { 111, 413 } },                           // closed by the end of another kind
            new object[] { v, new[] { 112, 412 } },
            new object[] { v, new[] { 111, 112, 412, 413 } },                 // crossed
            new object[] { v, new[] { 102, 403, 402, 404 } },                 // When after When Cancel
            new object[] { v, new[] { 102, 403, 403, 404 } },
            new object[] { v, new[] { 102, 404 } },                           // Show Choices without a When
            new object[] { v, new[] { 102, 230, 402, 404 } },                 // a command before the first When
            new object[] { v, new[] { 301, 603, 601, 604 } },                 // branches not in the order of Win, Escape, Lose
            new object[] { v, new[] { 301, 601, 601, 604 } },
            new object[] { v, new[] { 301, 602, 601, 604 } },
            new object[] { v, new[] { 111, 402, 412 } },                      // a branch of another block
            new object[] { v, new[] { 301, 601, 411, 604 } },
        });

    [Theory]
    [MemberData(nameof(Right))]
    public void ABlockStructureThatIsRightPasses(Version v, int[] codes) =>
        IndentHelper.ValidateStructure(EmptyCommandTests.EventOf(v, codes));

    [Theory]
    [MemberData(nameof(Wrong))]
    public void AStructureThatIsWrongIsReported(Version v, int[] codes) =>
        Assert.Throws<CommandStructureException>(() => IndentHelper.ValidateStructure(EmptyCommandTests.EventOf(v, codes)));

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheExceptionHasTheIndexAndTheCodeOfTheCommand(Version v)
    {
        var error = Assert.Throws<CommandStructureException>(() => IndentHelper.ValidateStructure(EmptyCommandTests.EventOf(v, 230, 111, 412, 412)));

        Assert.Equal(3, error.CommandIndex);
        Assert.Equal(412, error.CommandCode);
        Assert.Contains("412", error.Message);
        Assert.Contains("index 3", error.Message);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ForABlockThatIsNeverClosedItIsTheCommandThatOpensIt(Version v)
    {
        var error = Assert.Throws<CommandStructureException>(() => IndentHelper.ValidateStructure(EmptyCommandTests.EventOf(v, 230, 112, 111, 412)));

        Assert.Equal(1, error.CommandIndex);
        Assert.Equal(112, error.CommandCode);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AWrongIndentIsReportedWithTheCommandTheExpectedAndTheActualIndent(Version v)
    {
        var container = EmptyCommandTests.EventOf(v, 111, 230, 412);
        var storageVersion = v.Name == "MV" ? StorageVersion.MV : StorageVersion.VXAce;
        IndentHelper.SetupIndents(container, storageVersion);
        IndentHelper.ValidateIndents(container, storageVersion);

        container.NamedCommands[1].Indent = 0; // the command in the branch must be one level deeper
        var error = Assert.Throws<CommandIndentException>(() => IndentHelper.ValidateIndents(container, storageVersion));

        Assert.Equal(1, error.CommandIndex);
        Assert.Equal(230, error.CommandCode);
        Assert.Equal(1, error.ExpectedIndent);
        Assert.Equal(0, error.ActualIndent);
        Assert.Contains("index 1", error.Message);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ACommandWithoutAnIndentIsReportedToo(Version v)
    {
        var container = EmptyCommandTests.EventOf(v, 230);
        var storageVersion = v.Name == "MV" ? StorageVersion.MV : StorageVersion.VXAce;
        container.NamedCommands[0].Indent = null;

        var error = Assert.Throws<CommandIndentException>(() => IndentHelper.ValidateIndents(container, storageVersion));

        Assert.Null(error.ActualIndent);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void BothKindsOfErrorCanBeCaughtAsOne(Version v)
    {
        var broken = EmptyCommandTests.EventOf(v, 412);
        var storageVersion = v.Name == "MV" ? StorageVersion.MV : StorageVersion.VXAce;

        Assert.IsAssignableFrom<CommandListException>(Record.Exception(() => IndentHelper.ValidateStructure(broken)));
        Assert.IsAssignableFrom<CommandListException>(Record.Exception(() => IndentHelper.ValidateIndents(broken, storageVersion)));
    }
}
