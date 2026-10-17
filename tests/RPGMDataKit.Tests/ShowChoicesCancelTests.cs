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

    // the number that the maker writes for what Cancel does
    private static int Raw(Version v, CancelType type) => v.Name == "MV"
        ? (int)type
        : type switch
        {
            CancelType.Disallow => 0,
            CancelType.Branch => 5,
            _ => (int)type + 1,
        };

    private static ShowChoicesCommand InList(Version v, ShowChoicesCommand command)
    {
        EmptyCommandTests.EventOf(v).Append(b => b.Add(command));

        return command;
    }

    private static readonly CancelType[] InBoth =
        [CancelType.Disallow, CancelType.Branch, CancelType.Choice1, CancelType.Choice2, CancelType.Choice3, CancelType.Choice4];

    public static IEnumerable<object[]> Cancels() =>
        Version.All.SelectMany(v => InBoth.Select(type => new object[] { v, type }));

    [Theory]
    [MemberData(nameof(Cancels))]
    public void WhatIsSetBeforeTheCommandIsInAListGetsTheValueOfItsMaker(Version v, CancelType type)
    {
        var command = new ShowChoicesCommand { Choices = ["A", "B", "C", "D"], Cancel = type };

        InList(v, command);

        Assert.Equal(Raw(v, type), command.CancelChoiceIndex);
        Assert.Equal(type, command.Cancel);
        Assert.Equal(type == CancelType.Branch, command.IsCancelChoiceBranch);
    }

    [Theory]
    [MemberData(nameof(Cancels))]
    public void WhatIsSetInAListIsWrittenAtOnce(Version v, CancelType type)
    {
        var command = InList(v, new ShowChoicesCommand { Choices = ["A", "B", "C", "D"] });

        command.Cancel = type;

        Assert.Equal(Raw(v, type), command.CancelChoiceIndex);
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
    [InlineData(-1, CancelType.Disallow)]
    [InlineData(-2, CancelType.Branch)]
    [InlineData(0, CancelType.Choice1)]
    [InlineData(1, CancelType.Choice2)]
    [InlineData(3, CancelType.Choice4)]
    [InlineData(5, CancelType.Choice6)]
    public void TheRawValueOfMvIsRead(int raw, CancelType type)
    {
        var command = InList(Version.Mv, new ShowChoicesCommand { Choices = ["A"] });
        command.CancelChoiceIndex = raw;

        Assert.Equal(type, command.Cancel);
    }

    [Theory]
    [InlineData(0, CancelType.Disallow)]
    [InlineData(1, CancelType.Choice1)]
    [InlineData(2, CancelType.Choice2)]
    [InlineData(4, CancelType.Choice4)]
    [InlineData(5, CancelType.Branch)]
    public void TheRawValueOfVxAceIsRead(int raw, CancelType type)
    {
        var command = InList(Version.VXAce, new ShowChoicesCommand { Choices = ["A"] });
        command.CancelChoiceIndex = raw;

        Assert.Equal(type, command.Cancel);
    }

    [Fact]
    public void VxAceHasFourChoicesAndMvSix()
    {
        Assert.Equal(4, RawInList(Version.Mv, CancelType.Choice5));
        Assert.Equal(5, RawInList(Version.Mv, CancelType.Choice6));

        foreach (var type in new[] { CancelType.Choice5, CancelType.Choice6 })
        {
            Assert.Throws<DoesNotExistInVersionException>(() => InList(Version.VXAce, new ShowChoicesCommand { Choices = ["A"], Cancel = type }));

            var inVx = InList(Version.VXAce, new ShowChoicesCommand { Choices = ["A"] });
            Assert.Throws<DoesNotExistInVersionException>(() => inVx.Cancel = type);
        }
    }

    private static int RawInList(Version v, CancelType type) => InList(v, new ShowChoicesCommand { Choices = ["A"], Cancel = type }).CancelChoiceIndex;

    [Fact]
    public void ACommandOfVxAceCopiedToMvKeepsWhatCancelDoes()
    {
        foreach (var type in InBoth)
        {
            var vx = InList(Version.VXAce, new ShowChoicesCommand { Choices = ["A", "B", "C", "D"], Cancel = type });

            var mv = InList(Version.Mv, vx.Clone().Cast<ShowChoicesCommand>());

            Assert.Equal(type, mv.Cancel);
            Assert.Equal((int)type, mv.CancelChoiceIndex); // the number of MV is the number of the value
            Assert.Equal(["A", "B", "C", "D"], mv.Choices);
            // the window of a command of MV has what the editor gives: on the right, the first choice is the default one
            Assert.Equal((PositionType.Right, 0), (mv.Position, mv.DefaultChoiceIndex));
        }
    }

    [Fact]
    public void ACommandOfMvCopiedToVxAceHasOnlyTheChoicesAndTheCancel()
    {
        var mv = InList(Version.Mv, new ShowChoicesCommand { Choices = ["A", "B"], Cancel = CancelType.Choice2 });
        mv.Position = PositionType.Left;

        var vx = InList(Version.VXAce, mv.Clone().Cast<ShowChoicesCommand>());

        Assert.Equal(2, vx.Raw.Parameters!.Count);
        Assert.Equal(2, vx.CancelChoiceIndex);
        Assert.Equal(CancelType.Choice2, vx.Cancel);
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

    [Fact]
    public void APositionThatIsSetBeforeTheCommandIsInAListIsKept()
    {
        var left = InList(Version.Mv, new ShowChoicesCommand { Choices = ["A"], Position = PositionType.Left });
        var none = InList(Version.Mv, new ShowChoicesCommand { Choices = ["A"] });

        Assert.Equal(PositionType.Left, left.Position);
        Assert.Equal(PositionType.Right, none.Position);
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

        command.Cancel = CancelType.Choice3;
        Assert.Equal("Yes, No, Maybe, (#3)", Describe(Version.VXAce, command));

        command.Cancel = CancelType.Branch;
        Assert.Equal("Yes, No, Maybe, (-)", Describe(Version.VXAce, command));
    }

    [Fact]
    public void TheDumpOfMvHasTheWindowTheDefaultChoiceAndTheCancel()
    {
        var command = InList(Version.Mv, new ShowChoicesCommand { Choices = ["Yes", "No"] });
        Assert.Equal("Yes, No, (Window, Right, #1, -)", Describe(Version.Mv, command));

        command.Cancel = CancelType.Choice2;
        command.DefaultChoiceIndex = -1;
        Assert.Equal("Yes, No, (Window, Right, -, #2)", Describe(Version.Mv, command));
    }
}
