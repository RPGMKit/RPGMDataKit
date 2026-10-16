using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Exceptions;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The indents of the commands, which the engines use to find the end of a branch and the start of a loop:
/// <see cref="IndentHelper.SetupIndents"/> and <see cref="IndentHelper.ValidateIndents"/>. A list is written as the codes of the commands
/// (see <see cref="EmptyCommandTests"/>) with the indents that the editors give them.
/// </summary>
public sealed class IndentTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static readonly (string Name, int[] Codes, int[] Indents)[] Lists =
    [
        ("one command", [230], [0]),
        ("If with Else", [111, 230, 411, 230, 412, 230], [0, 1, 0, 1, 0, 0]),
        ("If with the empty commands", [111, 0, 411, 0, 412, 0], [0, 1, 0, 1, 0, 0]),
        ("Loop", [112, 230, 413], [0, 1, 0]),
        ("Show Choices with Cancel", [102, 402, 230, 402, 403, 230, 404], [0, 0, 1, 0, 0, 1, 0]),
        ("Battle Processing with all branches", [301, 601, 230, 602, 230, 603, 230, 604], [0, 0, 1, 0, 1, 0, 1, 0]),
        ("Battle Processing with Win and Escape", [301, 601, 230, 602, 230, 604], [0, 0, 1, 0, 1, 0]),
        ("Battle Processing without branches", [301, 230], [0, 0]),
        ("Battle Processing without branches in an If", [111, 301, 230, 412], [0, 1, 1, 0]),
        ("blocks inside each other", [111, 102, 402, 112, 230, 413, 404, 412, 230], [0, 1, 1, 2, 3, 2, 1, 0, 0]),
    ];

    public static IEnumerable<object[]> Cases() =>
        Version.All.SelectMany(v => Lists.Select(l => new object[] { v, l.Name, l.Codes, l.Indents }));

    private static StorageVersion StorageVersionOf(Version v) => v.Name == "MV" ? StorageVersion.MV : StorageVersion.VXAce;

    [Theory]
    [MemberData(nameof(Cases))]
    public void SetupGivesTheIndentsOfTheEditor(Version v, string name, int[] codes, int[] indents)
    {
        var container = EmptyCommandTests.EventOf(v, codes);
        foreach (var command in container.NamedCommands)
            command.Indent = 99; // whatever it was

        IndentHelper.SetupIndents(container, StorageVersionOf(v));

        Assert.Equal(indents, container.NamedCommands.Select(c => c.Indent!.Value).ToArray());
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ValidateAcceptsTheIndentsOfTheEditor(Version v, string name, int[] codes, int[] indents)
    {
        var container = EmptyCommandTests.EventOf(v, codes);
        for (var i = 0; i < indents.Length; i++)
            container.NamedCommands[i].Indent = indents[i];

        IndentHelper.ValidateIndents(container, StorageVersionOf(v));
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ValidateFindsTheFirstCommandWithAWrongIndent(Version v, string name, int[] codes, int[] indents)
    {
        // each command in turn gets an indent that is one more, and one less (where there is a less)
        for (var wrong = 0; wrong < indents.Length; wrong++)
        {
            foreach (var shift in new[] { 1, -1 })
            {
                var changed = indents[wrong] + shift;
                if (changed < 0)
                    continue;

                var container = EmptyCommandTests.EventOf(v, codes);
                for (var i = 0; i < indents.Length; i++)
                    container.NamedCommands[i].Indent = i == wrong ? changed : indents[i];

                var error = Assert.Throws<CommandIndentException>(() => IndentHelper.ValidateIndents(container, StorageVersionOf(v)));

                Assert.Equal(wrong, error.CommandIndex);
                Assert.Equal(codes[wrong], error.CommandCode);
                Assert.Equal(indents[wrong], error.ExpectedIndent);
                Assert.Equal(changed, error.ActualIndent);
            }
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void RandomListsOfTheEditorGetTheirIndentsAndPassTheCheck(Version v)
    {
        var random = new Random(20261017);
        for (var round = 0; round < 300; ++round)
        {
            var editor = EmptyCommandTests.EditorList(random);
            var container = EmptyCommandTests.EventOf(v, editor.Select(c => c.Code).ToArray());
            foreach (var command in container.NamedCommands)
                command.Indent = 99;

            IndentHelper.SetupIndents(container, StorageVersionOf(v));

            Assert.Equal(editor.Select(c => c.Indent).ToArray(), container.NamedCommands.Select(c => c.Indent!.Value).ToArray());
            IndentHelper.ValidateIndents(container, StorageVersionOf(v));
            IndentHelper.ValidateStructure(container);
        }
    }
}
