using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;
using static RPGMDataKit.Data.Structures.Commands.ShowChoicesCommand;

namespace RPGMDataKit.Tests;

/// <summary>
/// What Cancel of Show Choices does, the same for both makers: the raw value is -1/-2/n for MV (the choices from 0) and 0/5/n+1 for VX Ace.
/// The window, the position and the default choice are MV only.
/// </summary>
public sealed class ShowChoicesCancelTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static int Raw(Version v, CancelType type, int choice = 0) => (type, v.Name) switch
    {
        (CancelType.Disallow, "MV") => -1,
        (CancelType.Branch, "MV") => -2,
        (CancelType.Choice, "MV") => choice,
        (CancelType.Disallow, _) => 0,
        (CancelType.Branch, _) => 5,
        _ => choice + 1,
    };

    private static ShowChoicesCommand InList(Version v, ShowChoicesCommand command)
    {
        EmptyCommandTests.EventOf(v).AppendCommands(b => b.Add(command));

        return command;
    }

    public static IEnumerable<object[]> Cancels() =>
        Version.All.SelectMany(v => new[]
        {
            new object[] { v, CancelType.Disallow, 0 },
            new object[] { v, CancelType.Branch, 0 },
            new object[] { v, CancelType.Choice, 0 },
            new object[] { v, CancelType.Choice, 1 },
            new object[] { v, CancelType.Choice, 3 },
        });

    [Theory]
    [MemberData(nameof(Cancels))]
    public void WhatIsSetBeforeTheCommandIsInAListGetsTheValueOfItsMaker(Version v, CancelType type, int choice)
    {
        var command = new ShowChoicesCommand { Choices = ["A", "B", "C", "D"], Cancel = type };
        if (type == CancelType.Choice)
            command.CancelChoice = choice;

        InList(v, command);

        Assert.Equal(Raw(v, type, choice), command.CancelChoiceIndex);
        Assert.Equal(type, command.Cancel);
        Assert.Equal(type == CancelType.Choice ? choice : -1, command.CancelChoice);
        Assert.Equal(type == CancelType.Branch, command.IsCancelChoiceBranch);
    }

    [Theory]
    [MemberData(nameof(Cancels))]
    public void WhatIsSetInAListIsWrittenAtOnce(Version v, CancelType type, int choice)
    {
        var command = InList(v, new ShowChoicesCommand { Choices = ["A", "B", "C", "D"] });

        if (type == CancelType.Choice)
            command.CancelChoice = choice;
        else
            command.Cancel = type;

        Assert.Equal(Raw(v, type, choice), command.CancelChoiceIndex);
        Assert.Equal(type, command.Cancel);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ACommandThatIsNotToldAnythingDoesNotAllowCancel(Version v)
    {
        var command = InList(v, new ShowChoicesCommand { Choices = ["A"] });

        Assert.Equal(CancelType.Disallow, command.Cancel);
        Assert.Equal(Raw(v, CancelType.Disallow), command.CancelChoiceIndex);
    }

    [Theory]
    [InlineData(-1, CancelType.Disallow, -1)]
    [InlineData(-2, CancelType.Branch, -1)]
    [InlineData(0, CancelType.Choice, 0)]
    [InlineData(1, CancelType.Choice, 1)]
    [InlineData(3, CancelType.Choice, 3)]
    [InlineData(5, CancelType.Choice, 5)]
    public void TheRawValueOfMvIsRead(int raw, CancelType type, int choice)
    {
        var command = InList(Version.Mv, new ShowChoicesCommand { Choices = ["A"] });
        command.CancelChoiceIndex = raw;

        Assert.Equal((type, choice), (command.Cancel, command.CancelChoice));
    }

    [Theory]
    [InlineData(0, CancelType.Disallow, -1)]
    [InlineData(1, CancelType.Choice, 0)]
    [InlineData(2, CancelType.Choice, 1)]
    [InlineData(4, CancelType.Choice, 3)]
    [InlineData(5, CancelType.Branch, -1)]
    public void TheRawValueOfVxAceIsRead(int raw, CancelType type, int choice)
    {
        var command = InList(Version.VXAce, new ShowChoicesCommand { Choices = ["A"] });
        command.CancelChoiceIndex = raw;

        Assert.Equal((type, choice), (command.Cancel, command.CancelChoice));
    }

    [Fact]
    public void ACommandOfVxAceCopiedToMvKeepsWhatCancelDoes()
    {
        foreach (var (type, choice) in new[] { (CancelType.Disallow, -1), (CancelType.Branch, -1), (CancelType.Choice, 2) })
        {
            var vx = InList(Version.VXAce, new ShowChoicesCommand { Choices = ["A", "B", "C"] });
            vx.Cancel = type;
            if (choice >= 0)
                vx.CancelChoice = choice;

            var mv = InList(Version.Mv, vx.Clone().Cast<ShowChoicesCommand>());

            Assert.Equal((type, choice), (mv.Cancel, mv.CancelChoice));
            Assert.Equal(Raw(Version.Mv, type, Math.Max(choice, 0)), mv.CancelChoiceIndex);
            Assert.Equal(["A", "B", "C"], mv.Choices);
            // the window of a command of MV has what the editor gives: on the right, with a background, the first choice is the default one
            Assert.Equal((PositionType.Right, 0), (mv.Position, mv.DefaultChoiceIndex));
        }
    }

    [Fact]
    public void ACommandOfMvCopiedToVxAceHasOnlyTheChoicesAndTheCancel()
    {
        var mv = InList(Version.Mv, new ShowChoicesCommand { Choices = ["A", "B"] });
        mv.CancelChoice = 1;
        mv.Position = PositionType.Left;

        var vx = InList(Version.VXAce, mv.Clone().Cast<ShowChoicesCommand>());

        Assert.Equal(2, vx.Raw.Parameters!.Count);
        Assert.Equal(2, vx.CancelChoiceIndex);
        Assert.Equal(1, vx.CancelChoice);
    }

    [Fact]
    public void TheWindowAndTheDefaultChoiceExistInMvOnly()
    {
        var vx = InList(Version.VXAce, new ShowChoicesCommand { Choices = ["A"] });

        Assert.Throws<DoesNotExistInVersionException>(() => vx.Position);
        Assert.Throws<DoesNotExistInVersionException>(() => vx.Background);
        Assert.Throws<DoesNotExistInVersionException>(() => vx.DefaultChoiceIndex);
        Assert.Throws<DoesNotExistInVersionException>(() => vx.Position = PositionType.Left);

        var mv = InList(Version.Mv, new ShowChoicesCommand { Choices = ["A"] });
        mv.DefaultChoiceIndex = 1;
        mv.Position = PositionType.Middle;
        Assert.Equal((1, PositionType.Middle), (mv.DefaultChoiceIndex, mv.Position));
    }

    // ---- the text dump ----

    private static string Describe(Version v, ShowChoicesCommand command)
    {
        var project = Path.Combine(AppContext.BaseDirectory, "Projects", v.Name);
        Storage storage = v.Name == "MV" ? new MVStorage(project, "data") : new VXAceStorage(project, "Data");

        return command.GetParametersDescription(new EntityContext(storage), new DumpOptions());
    }

    [Fact]
    public void TheDumpOfVxAceHasTheChoicesAndTheCancelOnly()
    {
        var command = InList(Version.VXAce, new ShowChoicesCommand { Choices = ["Yes", "No", "Maybe"] });
        Assert.Equal("Yes, No, Maybe, (-)", Describe(Version.VXAce, command));

        command.CancelChoice = 2;
        Assert.Equal("Yes, No, Maybe, (#3)", Describe(Version.VXAce, command));

        command.Cancel = CancelType.Branch;
        Assert.Equal("Yes, No, Maybe, (-)", Describe(Version.VXAce, command));
    }

    [Fact]
    public void TheDumpOfMvHasTheWindowTheDefaultChoiceAndTheCancel()
    {
        var command = InList(Version.Mv, new ShowChoicesCommand { Choices = ["Yes", "No"] });
        Assert.Equal("Yes, No, (Window, Right, #1, -)", Describe(Version.Mv, command));

        command.CancelChoice = 1;
        command.DefaultChoiceIndex = -1;
        Assert.Equal("Yes, No, (Window, Right, -, #2)", Describe(Version.Mv, command));
    }
}
