using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.MovementCommands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The lines (the commands 505) after Set Movement Route are made from its route: the steps of the route are what counts, and
/// <see cref="RouteLinesHelper.SetupRouteLines"/> writes the lines again after they are changed.
/// </summary>
public sealed class RouteLinesTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static string Shape(ICommandContainer container) =>
        string.Join(" ", container.NamedCommands.Select(c => $"{c.Code}@{c.Indent}"));

    private static (ICommonEvent Container, SetMovementRouteCommand Move) WithARoute(Version v)
    {
        var container = EmptyCommandTests.EventOf(v);
        container.Replace(b => b.MoveRoute(r => r.Down().Wait(30)));

        return (container, container.NamedCommands.OfType<SetMovementRouteCommand>().Single());
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ARouteMadeByTheBuilderHasItsLines(Version v)
    {
        var (container, _) = WithARoute(v);

        Assert.Equal("205@0 505@0 505@0 0@0", Shape(container));
        RouteLinesHelper.ValidateRouteLines(container);
        Assert.Equal(0, RouteLinesHelper.SetupRouteLines(container) - 4); // the 2 old lines and the 2 new ones: nothing changes
        Assert.Equal("205@0 505@0 505@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AStepAddedToTheRouteIsNotInTheLinesUntilTheyAreSetUp(Version v)
    {
        var (container, move) = WithARoute(v);

        move.Route.NamedCommands.Insert(1, new MoveLeftCommand());

        var error = Assert.Throws<RouteLinesException>(() => RouteLinesHelper.ValidateRouteLines(container));
        Assert.Equal((2, 505), (error.CommandIndex, error.CommandCode)); // the second line is the wait, and the second step is the new step

        RouteLinesHelper.SetupRouteLines(container);

        Assert.Equal("205@0 505@0 505@0 505@0 0@0", Shape(container));
        Assert.Equal([1, 2, 15], container.NamedCommands.OfType<MovementRouteCommand>().Select(l => l.MoveCommand.Code));
        RouteLinesHelper.ValidateRouteLines(container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AStepRemovedFromTheRouteAndAStepThatIsChangedAreFoundAndFixed(Version v)
    {
        var (container, move) = WithARoute(v);

        ((MoveWaitCommand)move.Route.NamedCommands[1]).Frames = 99; // the step is changed, the line is not
        Assert.Throws<RouteLinesException>(() => RouteLinesHelper.ValidateRouteLines(container));
        RouteLinesHelper.SetupRouteLines(container);
        RouteLinesHelper.ValidateRouteLines(container);
        Assert.Equal(99, ((MoveWaitCommand)container.NamedCommands.OfType<MovementRouteCommand>().Last().MoveCommand).Frames);

        move.Route.NamedCommands.RemoveAt(0);
        Assert.Throws<RouteLinesException>(() => RouteLinesHelper.ValidateRouteLines(container));
        RouteLinesHelper.SetupRouteLines(container);
        Assert.Equal("205@0 505@0 0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ALineThatIsChangedItselfIsFoundAndWrittenAgainFromTheRoute(Version v)
    {
        var (container, _) = WithARoute(v);

        ((MoveWaitCommand)container.NamedCommands.OfType<MovementRouteCommand>().Last().MoveCommand).Frames = 7;

        Assert.Throws<RouteLinesException>(() => RouteLinesHelper.ValidateRouteLines(container));
        RouteLinesHelper.SetupRouteLines(container);
        RouteLinesHelper.ValidateRouteLines(container);
        Assert.Equal(30, ((MoveWaitCommand)container.NamedCommands.OfType<MovementRouteCommand>().Last().MoveCommand).Frames); // the route counts
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ALineWithoutItsCommandIsRemoved(Version v)
    {
        var (container, move) = WithARoute(v);

        container.NamedCommands.Remove(move);

        var error = Assert.Throws<RouteLinesException>(() => RouteLinesHelper.ValidateRouteLines(container));
        Assert.Equal((0, 505), (error.CommandIndex, error.CommandCode));
        Assert.Equal(2, RouteLinesHelper.SetupRouteLines(container));
        Assert.Equal("0@0", Shape(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheLinesOfEachRouteStayWithItsOwnCommandAndHaveItsIndent(Version v)
    {
        var container = EmptyCommandTests.EventOf(v);
        container.Replace(b => b
            .MoveRoute(r => r.Down())
            .IfSwitch(1, then: t => t.MoveRoute(r => r.Up().Left())));
        Assert.Equal("205@0 505@0 111@0 205@1 505@1 505@1 0@1 412@0 0@0", Shape(container));

        var second = container.NamedCommands.OfType<SetMovementRouteCommand>().Last();
        second.Route.NamedCommands.Insert(0, new MoveRightCommand());
        RouteLinesHelper.SetupRouteLines(container);

        Assert.Equal("205@0 505@0 111@0 205@1 505@1 505@1 505@1 0@1 412@0 0@0", Shape(container));
        RouteLinesHelper.ValidateRouteLines(container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ARouteWithNoStepsHasNoLines(Version v)
    {
        var (container, move) = WithARoute(v);

        move.Route.NamedCommands.Clear();
        move.Route.NamedCommands.Add(new MoveEmptyCommand());
        RouteLinesHelper.SetupRouteLines(container);

        Assert.Equal("205@0 0@0", Shape(container));
        RouteLinesHelper.ValidateRouteLines(container);
    }
}
