using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.IfCommands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Internal;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// Commands whose parameters the two makers write differently: a property says what the parameter means in both, and the raw parameter is the
/// one of the maker of the list the command is put into. Used by a converter (a command of VX Ace is copied with its meaning into a list of MV).
/// </summary>
public sealed class CommandMeaningTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static bool IsMv(Version v) => v.Name == "MV";

    private static T InList<T>(Version v, T command) where T : NamedCommand
    {
        EmptyCommandTests.EventOf(v).Append(b => b.Add(command));

        return command;
    }

    private static int RawInt(NamedCommand command, int index) => Convert.ToInt32(command.Raw.Parameters![index]);

    // ---- 104 Select Item ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void SelectItemHasTheTypeInMvOnly(Version v)
    {
        var command = InList(v, new SelectItemCommand { VariableId = 15 });

        Assert.Equal(15, command.VariableId);
        Assert.Equal(IsMv(v) ? 2 : 1, command.Raw.Parameters!.Count);
        if (IsMv(v))
            Assert.Equal(SelectItemCommand.KeyItemType.Regular, command.Type); // what the editor gives to a new command
        else
            Assert.Throws<DoesNotExistInVersionException>(() => command.Type);
    }

    [Fact]
    public void SelectItemTypeIsKept()
    {
        var command = InList(Version.Mv, new SelectItemCommand { VariableId = 15, Type = SelectItemCommand.KeyItemType.Key });

        Assert.Equal(2, RawInt(command, 1));
    }

    [Fact]
    public void SelectItemOfMvPutIntoVxAceLosesTheType()
    {
        var mv = InList(Version.Mv, new SelectItemCommand { VariableId = 3, Type = SelectItemCommand.KeyItemType.HiddenA });

        var vx = InList(Version.VXAce, mv.Clone().Cast<SelectItemCommand>());

        Assert.Equal(1, vx.Raw.Parameters!.Count);
        Assert.Equal(3, vx.VariableId);
    }

    // ---- 285 Get Location Info ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheRegionIsTheSameInfoInBothMakers(Version v)
    {
        var command = InList(v, new GetLocationInfoCommand { VariableId = 1, Type = GetLocationInfoCommand.InfoType.RegionI });

        Assert.Equal(IsMv(v) ? 6 : 5, RawInt(command, 1));
        Assert.Equal(GetLocationInfoCommand.InfoType.RegionI, command.Type);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheOtherInfoKeepsItsNumber(Version v)
    {
        foreach (var type in new[]
                 {
                     GetLocationInfoCommand.InfoType.TerrainTag, GetLocationInfoCommand.InfoType.EventId,
                     GetLocationInfoCommand.InfoType.TileIdLayer1, GetLocationInfoCommand.InfoType.TileIdLayer3,
                 })
        {
            var command = InList(v, new GetLocationInfoCommand { Type = type });

            Assert.Equal((int)type, RawInt(command, 1));
            Assert.Equal(type, command.Type);
        }
    }

    [Fact]
    public void TheFourthLayerOfTilesExistsInMvOnly()
    {
        var mv = InList(Version.Mv, new GetLocationInfoCommand { Type = GetLocationInfoCommand.InfoType.TileIdLayer4 });

        Assert.Equal(5, RawInt(mv, 1));
        Assert.Throws<DoesNotExistInVersionException>(() => InList(Version.VXAce, new GetLocationInfoCommand { Type = GetLocationInfoCommand.InfoType.TileIdLayer4 }));
        Assert.Throws<DoesNotExistInVersionException>(() => InList(Version.VXAce, mv.Clone().Cast<GetLocationInfoCommand>()));
    }

    [Fact]
    public void ARegionCopiedFromVxAceToMvIsStillTheRegion()
    {
        var vx = InList(Version.VXAce, new GetLocationInfoCommand { Type = GetLocationInfoCommand.InfoType.RegionI });
        Assert.Equal(5, RawInt(vx, 1));

        var mv = InList(Version.Mv, vx.Clone().Cast<GetLocationInfoCommand>());

        Assert.Equal(6, RawInt(mv, 1));
        Assert.Equal(GetLocationInfoCommand.InfoType.RegionI, mv.Type);
    }

    // ---- 319 Change Equipment ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheSlotIsCountedFromZeroInBothMakers(Version v)
    {
        var command = InList(v, new ChangeEquipmentCommand { ActorId = 1, SlotIndex = 1, ItemId = 9 });

        Assert.Equal(IsMv(v) ? 2 : 1, RawInt(command, 1)); // MV counts the slots from 1
        Assert.Equal(1, command.SlotIndex);
        Assert.Equal(IsMv(v) ? 2 : 1, command.TypeId);

        command.SlotIndex = 3;
        Assert.Equal(IsMv(v) ? 4 : 3, RawInt(command, 1));
        Assert.Equal(3, command.SlotIndex);
    }

    [Fact]
    public void ASlotCopiedBetweenTheMakersIsTheSameSlot()
    {
        var vx = InList(Version.VXAce, new ChangeEquipmentCommand { ActorId = 1, SlotIndex = 3, ItemId = 5 });

        var mv = InList(Version.Mv, vx.Clone().Cast<ChangeEquipmentCommand>());
        Assert.Equal((4, 3), (RawInt(mv, 1), mv.SlotIndex));

        var back = InList(Version.VXAce, mv.Clone().Cast<ChangeEquipmentCommand>());
        Assert.Equal((3, 3), (RawInt(back, 1), back.SlotIndex));
    }

    [Fact]
    public void TheRawNumberOfTheSlotIsKeptWhenItIsSetInAList()
    {
        // the number as it is written, for a command whose maker is known
        var mv = InList(Version.Mv, new ChangeEquipmentCommand { ActorId = 1, ItemId = 5 });
        mv.TypeId = 2;

        Assert.Equal(2, RawInt(mv, 1));
        Assert.Equal(1, mv.SlotIndex);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ASlotThatIsNotToldIsTheFirstOne(Version v)
    {
        var command = InList(v, new ChangeEquipmentCommand { ActorId = 1, ItemId = 5 });

        Assert.Equal(0, command.SlotIndex);
        Assert.Equal(IsMv(v) ? 1 : 0, RawInt(command, 1));
    }

    // ---- 111 If, a button ----

    [Fact]
    public void AButtonIsANumberInVxAceAndANameInMv()
    {
        var vx = InList(Version.VXAce, new IfButtonCommand { Button = IfButtonCommand.Buttons.B });
        Assert.Equal(12, RawInt(vx, 1));

        var mv = InList(Version.Mv, new IfButtonCommand { Button = IfButtonCommand.Buttons.B });
        Assert.Equal("cancel", mv.Raw.Parameters![1]);
        Assert.Equal(IfButtonCommand.Buttons.B, mv.Button);
    }

    [Fact]
    public void AButtonCopiedFromVxAceToMvGetsTheName()
    {
        foreach (var (button, name) in new[]
                 {
                     (IfButtonCommand.Buttons.Down, "down"), (IfButtonCommand.Buttons.A, "shift"), (IfButtonCommand.Buttons.C, "ok"),
                     (IfButtonCommand.Buttons.L, "pageup"), (IfButtonCommand.Buttons.R, "pagedown"),
                 })
        {
            var vx = InList(Version.VXAce, new IfButtonCommand { Button = button });
            var mv = InList(Version.Mv, vx.Clone().Cast<IfButtonCommand>());

            Assert.Equal(name, mv.Raw.Parameters![1]);
            Assert.Equal(button, mv.Button);
            Assert.Equal(name, vx.MvName);
        }
    }

    [Fact]
    public void TheButtonsXyzHaveNoNameInMv()
    {
        foreach (var button in new[] { IfButtonCommand.Buttons.X, IfButtonCommand.Buttons.Y, IfButtonCommand.Buttons.Z })
            Assert.Null(InList(Version.VXAce, new IfButtonCommand { Button = button }).MvName);
    }

    // ---- 302 / 605 Shop Processing ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheFirstLineOfAShopHasFiveParametersAndTheNextOnesFour(Version v)
    {
        var first = InList(v, new ShopProcessingCommand { Type = ShopProcessingCommand.ItemType.Weapon, ItemId = 4, PurchaseOnly = true });
        var line = InList(v, new ShopProcessingLineCommand { Type = ShopProcessingCommand.ItemType.Armor, ItemId = 6, HasCustomPrice = true, CustomPrice = 70 });

        Assert.Equal(5, first.Raw.Parameters!.Count);
        Assert.True(first.PurchaseOnly);
        Assert.Equal(4, line.Raw.Parameters!.Count);
        Assert.Equal((70, true), (line.CustomPrice, line.HasCustomPrice));
        Assert.Equal([2, 6, 1, 70], line.Raw.Parameters.Select(Convert.ToInt32)); // the type (armor), the item, the flag of the price, the price
    }
}
