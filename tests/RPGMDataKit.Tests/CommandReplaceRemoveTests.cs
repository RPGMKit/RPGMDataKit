using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// <c>ReplaceAt</c> and <c>RemoveAt</c> of <see cref="CommandContainerExtensions"/>, and the overloads that take commands made by hand. A command goes
/// with all it holds (the lines of a text, an If with its branches, a route with its lines); the places that are not places for a command, and the
/// empty command that ends a block, are refused.
/// </summary>
public sealed class CommandReplaceRemoveTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static ICommonEvent Empty(Version v) => EmptyCommandTests.EventOf(v);

    private static string Shape(ICommandContainer container) =>
        string.Join(" ", container.NamedCommands.Select(c => $"{c.Code}@{c.Indent}"));

    // ---- RemoveAt ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void ASingleCommandIsRemoved(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Add(new WaitCommand(), new WaitCommand { FrameCount = 5 }, new WaitCommand()));

        container.RemoveAt(1);

        Assert.Equal("230@0 230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ACommandOfSeveralLinesIsRemovedWithItsLines(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Add(new WaitCommand()).Text("a", "b").Comment("c", "d").MoveRoute(r => r.Down().Up()).Shop(s => s.Item(1).Item(2)));
        Assert.Equal("230@0 101@0 401@0 401@0 108@0 408@0 205@0 505@0 505@0 302@0 605@0 0@0", Shape(container));

        container.RemoveAt(1);   // the text
        Assert.Equal("230@0 108@0 408@0 205@0 505@0 505@0 302@0 605@0 0@0", Shape(container));

        container.RemoveAt(3);   // the route with its lines
        Assert.Equal("230@0 108@0 408@0 302@0 605@0 0@0", Shape(container));

        container.RemoveAt(3);   // the shop
        container.RemoveAt(1);   // the comment
        Assert.Equal("230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABlockIsRemovedWithEverythingInIt(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b
            .Add(new WaitCommand())
            .IfSwitch(1,
                then: t => t.Loop(l => l.IfSwitch(2, then: i => i.BreakLoop())).Choices(c => c.When("A", a => a.Add(new WaitCommand())).Cancel()),
                @else: e => e.Battle(1, escape: _ => { }))
            .Add(new WaitCommand()));

        container.RemoveAt(1);

        Assert.Equal("230@0 230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABlockInsideABlockIsRemovedAlone(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.IfSwitch(1, then: t => t.Add(new WaitCommand()).Loop(l => l.Add(new WaitCommand())).Add(new WaitCommand())));
        // 111, 230, 112, 230, 0, 413, 230, 0, 412, 0

        container.RemoveAt(2);   // the loop

        Assert.Equal("111@0 230@1 230@1 0@1 412@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABattleWithoutBranchesIsOneCommand(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Battle(1).Add(new WaitCommand()));

        container.RemoveAt(0);

        Assert.Equal("230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void RemovingTheOnlyCommandOfABranchLeavesTheBranchWithItsEmptyCommand(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.IfSwitch(1, then: t => t.Add(new WaitCommand())));

        container.RemoveAt(1);

        Assert.Equal("111@0 0@1 412@0 0@0", Shape(container));
    }

    // ---- what is refused ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheEmptyCommandThatEndsABlockOrTheListIsNotRemovedOrReplaced(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.IfSwitch(1, then: t => t.Add(new WaitCommand())));
        // 111, 230, 0, 412, 0
        var before = Shape(container);

        var removed = Assert.Throws<CommandPlaceException>(() => container.RemoveAt(2));
        Assert.Equal((2, 0), (removed.CommandIndex, removed.CommandCode));
        Assert.Contains("InsertAt(2, ...)", removed.Message);
        Assert.Throws<CommandPlaceException>(() => container.RemoveAt(4));                       // the end of the list
        Assert.Throws<CommandPlaceException>(() => container.ReplaceAt(2, b => b.Comment("x")));
        Assert.Equal(before, Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheLinesAndWhatDividesOrEndsABlockAreNotRemovedOrReplaced(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Text("a", "b").IfSwitch(1, then: t => t.Add(new WaitCommand()), @else: e => e.Add(new WaitCommand())));
        // 101, 401, 401, 111, 230, 0, 411, 230, 0, 412, 0
        var before = Shape(container);

        foreach (var index in new[] { 1, 2, 6, 9 })   // the lines, the Else and the Branch End
        {
            Assert.Throws<CommandPlaceException>(() => container.RemoveAt(index));
            Assert.Throws<CommandPlaceException>(() => container.ReplaceAt(index, b => b.Comment("x")));
        }

        Assert.Equal(before, Shape(container));
        var line = Assert.Throws<CommandPlaceException>(() => container.RemoveAt(2));
        Assert.Contains("Use 0", line.Message);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AnIndexThatIsNotInTheListIsAMistake(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Add(new WaitCommand()));

        Assert.Throws<ArgumentOutOfRangeException>(() => container.RemoveAt(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => container.RemoveAt(container.NamedCommands.Count));
        Assert.Throws<ArgumentOutOfRangeException>(() => container.ReplaceAt(container.NamedCommands.Count, b => b.Comment("x")));
    }

    // ---- ReplaceAt ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void ACommandIsReplacedByCommandsOfTheBuilder(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Add(new WaitCommand()).Add(new WaitCommand { FrameCount = 5 }).Add(new WaitCommand()));

        container.ReplaceAt(1, b => b.Text("a", "b").IfSwitch(3));

        Assert.Equal("230@0 101@0 401@0 401@0 111@0 0@1 412@0 230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABlockIsReplacedWithAllItHolds(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Add(new WaitCommand()).IfSwitch(1, then: t => t.Text("x"), @else: e => e.Loop()).Add(new WaitCommand()));

        container.ReplaceAt(1, b => b.Comment("only this"));

        Assert.Equal("230@0 108@0 230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ACommandOfSeveralLinesIsReplacedWithItsLines(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.MoveRoute(r => r.Down().Up()).Add(new WaitCommand()));

        container.ReplaceAt(0, b => b.Add(new WaitCommand()));

        Assert.Equal("230@0 230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ReplacingWithNothingIsRemoving(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.Add(new WaitCommand()).Text("a").Add(new WaitCommand()));

        container.ReplaceAt(1, _ => { });

        Assert.Equal("230@0 230@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABlockCanBeReplacedByARouteAndItsLinesAreMade(Version v)
    {
        var container = Empty(v);
        container.Replace(b => b.IfSwitch(1));

        container.ReplaceAt(0, b => b.MoveRoute(r => r.Down()));

        Assert.Equal("205@0 505@0 0@0", Shape(container));
        RouteLinesHelper.ValidateRouteLines(container);
    }

    // ---- the overloads that take commands made by hand ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void CommandsMadeByHandAreAppendedInsertedAndReplaced(Version v)
    {
        var container = Empty(v);

        container.Replace(new WaitCommand(), new WaitCommand { FrameCount = 5 });
        Assert.Equal("230@0 230@0 0@0", Shape(container));

        container.Append(new ShowTextBaseCommand(), new ShowTextLineCommand { Text = "hello" });
        Assert.Equal("230@0 230@0 101@0 401@0 0@0", Shape(container));

        container.InsertAt(1, new WaitCommand { FrameCount = 7 });
        Assert.Equal("230@0 230@0 230@0 101@0 401@0 0@0", Shape(container));
        Assert.Equal(7, ((WaitCommand)container.NamedCommands[1]).FrameCount);

        container.ReplaceAt(3, new CommentCommand { Comment = "c" });   // the text with its lines
        Assert.Equal("230@0 230@0 230@0 108@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void CommandsMadeByHandAreCheckedLikeThoseOfTheBuilder(Version v)
    {
        var container = Empty(v);
        container.Replace(new WaitCommand(), new ShowTextBaseCommand(), new ShowTextLineCommand { Text = "a" });

        Assert.Throws<CommandPlaceException>(() => container.InsertAt(2, new WaitCommand()));   // a line of the text
        Assert.Throws<ArgumentNullException>(() => container.Append((RPGMDataKit.Data.Structures.Internal.NamedCommand[])null!));
        Assert.Throws<ArgumentNullException>(() => container.Append(new WaitCommand(), null!));
    }
}
