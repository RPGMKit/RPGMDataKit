using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// <see cref="CommandContainerExtensions.InsertAt"/> puts commands where the editors do: in place of the command with the index, which goes down.
/// The lines of a command of several lines and what divides or ends a block (When, Else, Branch End...) are not places for a command; the end of a
/// block is its empty command (the commands go before it, into the block).
/// </summary>
public sealed class CommandPlaceTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static ICommonEvent Empty(Version v) => EmptyCommandTests.EventOf(v);

    private static string Shape(ICommandContainer container) =>
        string.Join(" ", container.NamedCommands.Select(c => $"{c.Code}@{c.Indent}"));

    // every kind of command that is not a place, in a list made by the builder: the code, the builder, the index of the command
    public static IEnumerable<object[]> NotPlaces()
    {
        foreach (var v in Version.All)
        {
            yield return new object[] { v, 401, (Action<CommandBuilder>)(b => b.Text("a", "b")), 1 };
            yield return new object[] { v, 405, (Action<CommandBuilder>)(b => b.ScrollingText("a", "b")), 1 };
            yield return new object[] { v, 408, (Action<CommandBuilder>)(b => b.Comment("a", "b")), 1 };
            yield return new object[] { v, 655, (Action<CommandBuilder>)(b => b.Script("a", "b")), 1 };
            yield return new object[] { v, 605, (Action<CommandBuilder>)(b => b.Shop(s => s.Item(1).Item(2))), 1 };
            yield return new object[] { v, 505, (Action<CommandBuilder>)(b => b.MoveRoute(r => r.Down())), 1 };
            yield return new object[] { v, 402, (Action<CommandBuilder>)(b => b.Choices(c => c.When("A").When("B"))), 3 };
            yield return new object[] { v, 403, (Action<CommandBuilder>)(b => b.Choices(c => c.When("A").Cancel())), 3 };
            yield return new object[] { v, 404, (Action<CommandBuilder>)(b => b.Choices(c => c.When("A"))), 3 };
            yield return new object[] { v, 411, (Action<CommandBuilder>)(b => b.IfSwitch(1, @else: _ => { })), 2 };
            yield return new object[] { v, 412, (Action<CommandBuilder>)(b => b.IfSwitch(1)), 2 };
            yield return new object[] { v, 413, (Action<CommandBuilder>)(b => b.Loop()), 2 };
            yield return new object[] { v, 601, (Action<CommandBuilder>)(b => b.Battle(1, escape: _ => { })), 1 };
            yield return new object[] { v, 602, (Action<CommandBuilder>)(b => b.Battle(1, escape: _ => { })), 3 };
            yield return new object[] { v, 603, (Action<CommandBuilder>)(b => b.Battle(1, lose: _ => { })), 3 };
            yield return new object[] { v, 604, (Action<CommandBuilder>)(b => b.Battle(1, escape: _ => { })), 5 };
        }
    }

    [Theory]
    [MemberData(nameof(NotPlaces))]
    public void AnIndexOfALineOrOfWhatDividesOrEndsABlockIsRefused(Version v, int code, Action<CommandBuilder> build, int index)
    {
        var container = Empty(v);
        container.Replace(build);
        Assert.Equal(code, container.NamedCommands[index].Code); // the test points at the command it means
        var before = Shape(container);

        var error = Assert.Throws<CommandPlaceException>(() => container.InsertAt(index, b => b.Add(new WaitCommand())));

        Assert.Equal((index, code), (error.CommandIndex, error.CommandCode));
        Assert.Equal(before, Shape(container)); // nothing was put
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheMessageSaysWhereToPutTheCommandsInstead(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Add(new WaitCommand()).Text("a", "b", "c").Add(new WaitCommand()));
        // 230, 101, 401, 401, 401, 230, 0

        var line = Assert.Throws<CommandPlaceException>(() => container.InsertAt(3, _ => { }));
        Assert.Contains("Use 1 (before the command) or 5 (after its lines)", line.Message);

        var blocks = Empty(v);
        blocks.Replace(b => b.IfSwitch(1, then: t => t.Add(new WaitCommand())));
        // 111, 230, 0, 412, 0
        var closer = Assert.Throws<CommandPlaceException>(() => blocks.InsertAt(3, _ => { }));
        Assert.Contains("Use 2 (the empty command that ends the block above", closer.Message);
        Assert.Contains("or 4 (after it)", closer.Message);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheHeadOfACommandOfSeveralLinesIsAPlaceAndSoIsTheCommandAfterItsLines(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Text("a", "b"));
        // 101, 401, 401, 0

        container.InsertAt(0, b => b.Add(new WaitCommand())); // before the text
        Assert.Equal("230@0 101@0 401@0 401@0 0@0", Shape(container));

        container.InsertAt(4, b => b.Add(new WaitCommand())); // right after the lines of the text (the empty command that ends the list)
        Assert.Equal("230@0 101@0 401@0 401@0 230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheEmptyCommandThatEndsABlockIsThePlaceForTheEndOfTheBlock(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.IfSwitch(1, then: t => t.Add(new WaitCommand()), @else: e => e.Add(new WaitCommand())));
        // 111, 230, 0, 411, 230, 0, 412, 0

        container.InsertAt(2, b => b.Text("end of Then"));     // the empty command of the Then branch
        container.InsertAt(7, b => b.Text("end of Else"));     // the empty command of the Else branch (the text moved it down by two)

        Assert.Equal("111@0 230@1 101@1 401@1 0@1 411@0 230@1 101@1 401@1 0@1 412@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMovementRouteIsAPlaceAtItsHead(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.MoveRoute(r => r.Down().Up()));
        // 205, 505, 505, 0

        container.InsertAt(0, b => b.Add(new WaitCommand()));

        Assert.Equal("230@0 205@0 505@0 505@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheLengthOfTheListIsTheEndOfIt(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Add(new WaitCommand()));

        container.InsertAt(container.NamedCommands.Count, b => b.Add(new WaitCommand()));

        Assert.Equal("230@0 230@0 0@0", Shape(container));
    }
}
