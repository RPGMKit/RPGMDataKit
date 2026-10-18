using RPGMDataKit.Data.Enums;
using RPGMDataKit.Storages;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.IfCommands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
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
        var command = InList(v, new GetLocationInfoCommand { VariableId = 1, Type = GetLocationInfoCommand.InfoType.Region });

        Assert.Equal(IsMv(v) ? 6 : 5, RawInt(command, 1));
        Assert.Equal(GetLocationInfoCommand.InfoType.Region, command.Type);
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
        var vx = InList(Version.VXAce, new GetLocationInfoCommand { Type = GetLocationInfoCommand.InfoType.Region });
        Assert.Equal(5, RawInt(vx, 1));

        var mv = InList(Version.Mv, vx.Clone().Cast<GetLocationInfoCommand>());

        Assert.Equal(6, RawInt(mv, 1));
        Assert.Equal(GetLocationInfoCommand.InfoType.Region, mv.Type);
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

    // ---- the parameters that the editors write, and nothing else ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheConditionAboutAnActorHasTheParameterOfItsKindOnly(Version v)
    {
        var inParty = InList(v, new IfActorCommand { ActorId = 4, Check = IfActorCommand.CheckType.InParty });
        var name = InList(v, new IfActorCommand { ActorId = 4, Check = IfActorCommand.CheckType.Name, Param = "Eric" });
        var state = InList(v, new IfActorCommand { ActorId = 4, Check = IfActorCommand.CheckType.State, Param = 7 });

        Assert.Equal(3, inParty.Raw.Parameters!.Count);   // [4, actor, 0]
        Assert.Equal(4, name.Raw.Parameters!.Count);
        Assert.Equal(4, state.Raw.Parameters!.Count);

        name.Check = IfActorCommand.CheckType.InParty;      // changed in a list
        Assert.Equal(3, name.Raw.Parameters.Count);
        name.Check = IfActorCommand.CheckType.Skill;
        name.Param = 9;
        Assert.Equal([4, 4, 3, 9], name.Raw.Parameters.Select(Convert.ToInt32));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheConditionAboutAnEnemyHasTheParameterOfItsKindOnly(Version v)
    {
        var appeared = InList(v, new IfEnemyCommand { TroopId = 1, Check = IfEnemyCommand.CheckType.Appeared });
        var state = InList(v, new IfEnemyCommand { TroopId = 1, Check = IfEnemyCommand.CheckType.State, Param = 5 });

        // VX Ace (looked at in the editor): [5, 0, 0, 1], the state 1 is the one of the disabled list; MV (not looked at): no fourth parameter
        Assert.Equal(IsMv(v) ? 3 : 4, appeared.Raw.Parameters!.Count);
        if (!IsMv(v))
            Assert.Equal(1, Convert.ToInt32(appeared.Raw.Parameters[3]));
        Assert.Equal(4, state.Raw.Parameters!.Count);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void GameDataOfAVariableHasSevenParametersWhateverTheKind(Version v)
    {
        foreach (var kind in new[] { ControlVariableCommand.GameDataType.ItemInInventory, ControlVariableCommand.GameDataType.Actor, ControlVariableCommand.GameDataType.Character, ControlVariableCommand.GameDataType.Other })
        {
            var command = InList(v, new ControlVariableCommand
            {
                StartVariable = 15, EndVariable = 15, Operand = ControlVariableCommand.OperandType.GameData, Value1 = (int)kind,
            });

            Assert.Equal(7, command.Raw.Parameters!.Count);
            Assert.Equal([0, 0], command.Raw.Parameters.Skip(5).Select(Convert.ToInt32)); // what is not told is 0, as in the files
        }

        Assert.Equal(6, InList(v, new ControlVariableCommand { Operand = ControlVariableCommand.OperandType.Random, Value1 = 1, Value2 = 6 }).Raw.Parameters!.Count);
        Assert.Equal(5, InList(v, new ControlVariableCommand { Operand = ControlVariableCommand.OperandType.Constant, Value1 = 3 }).Raw.Parameters!.Count);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void StoppingTheTimerHasNoSeconds(Version v)
    {
        var start = InList(v, new ControlTimerCommand { State = OnOffState.On, Seconds = 90 });
        var stop = InList(v, new ControlTimerCommand { State = OnOffState.Off });

        Assert.Equal([0, 90], start.Raw.Parameters!.Select(Convert.ToInt32));
        Assert.Equal([1], stop.Raw.Parameters!.Select(Convert.ToInt32));

        stop.State = OnOffState.On;
        stop.Seconds = 10;
        Assert.Equal([0, 10], stop.Raw.Parameters!.Select(Convert.ToInt32));
        stop.State = OnOffState.Off;
        Assert.Equal([1], stop.Raw.Parameters!.Select(Convert.ToInt32));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheSecondParameterOfMovePictureIsZeroInMvAndNilInVxAce(Version v)
    {
        var command = InList(v, new MovePictureCommand { Layer = 3, Frames = 30 });

        if (IsMv(v))
            Assert.Equal(0, command.Raw.Parameters![1]);
        else
            Assert.Null(command.Raw.Parameters![1]);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANewCommandAboutAnItemIsForTheFirstOneAndTheCountOne(Version v)
    {
        var item = InList(v, new ChangeItemsCommand());
        var weapon = InList(v, new ChangeWeaponsCommand());
        var armor = InList(v, new ChangeArmorsCommand());

        Assert.Equal([1, 0, 0, 1], item.Raw.Parameters!.Select(Convert.ToInt32));
        Assert.Equal([1, 0, 0, 1, 0], weapon.Raw.Parameters!.Select(Convert.ToInt32));
        Assert.Equal([1, 0, 0, 1, 0], armor.Raw.Parameters!.Select(Convert.ToInt32));

        // what is set stays, zero too
        var zero = InList(v, new ChangeArmorsCommand { ArmorId = 0, Value = 0 });
        Assert.Equal([0, 0, 0, 0, 0], zero.Raw.Parameters!.Select(Convert.ToInt32));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void IncludingTheEquipmentIsOnlyForTakingWeaponsAndArmors(Version v)
    {
        var take = InList(v, new ChangeArmorsCommand { Operator = ChangeArmorsCommand.OperatorType.Take, IncludeEquipment = true });
        var add = InList(v, new ChangeArmorsCommand { Operator = ChangeArmorsCommand.OperatorType.Add, IncludeEquipment = true });
        var weapon = InList(v, new ChangeWeaponsCommand { IncludeEquipment = true, Operator = ChangeWeaponsCommand.OperatorType.Add });

        Assert.True(take.IncludeEquipment);
        Assert.Equal(1, Convert.ToInt32(take.Raw.Parameters![4]));
        Assert.False(add.IncludeEquipment);
        Assert.Equal(0, Convert.ToInt32(add.Raw.Parameters![4]));   // written as the editors write it
        Assert.False(weapon.IncludeEquipment);
        Assert.Equal(0, Convert.ToInt32(weapon.Raw.Parameters![4]));

        // the order of the properties does not matter
        var ordered = InList(v, new ChangeWeaponsCommand { IncludeEquipment = true, Operator = ChangeWeaponsCommand.OperatorType.Take });
        Assert.True(ordered.IncludeEquipment);
    }

    [Fact]
    public void ANewSoundOfVxAceHasTheVolumeTheEditorGives()
    {
        // read from the files that the editor of VX Ace wrote: BGM and ME 100, SE and BGS 80, the pitch 100
        var se = InList(Version.VXAce, new PlaySECommand());
        var bgs = InList(Version.VXAce, new PlayBGSCommand());
        var bgm = InList(Version.VXAce, new PlayBGMCommand());
        var me = InList(Version.VXAce, new PlayMECommand());

        Assert.Equal((80, 100), (se.SoundData.Volume, se.SoundData.Pitch));
        Assert.Equal((80, 100), (bgs.SoundData.Volume, bgs.SoundData.Pitch));
        Assert.Equal((100, 100), (bgm.SoundData.Volume, bgm.SoundData.Pitch));
        Assert.Equal((100, 100), (me.SoundData.Volume, me.SoundData.Pitch));

        // what is set stays
        var own = Version.VXAce.New<ISE>();
        own.Volume = 55;
        Assert.Equal(55, InList(Version.VXAce, new PlaySECommand { SoundData = own }).SoundData.Volume);
    }

    [Fact]
    public void TheTypeMapKnowsItsMaker()
    {
        Assert.Equal(StorageVersion.MV, TypeMap.Mv.Version);
        Assert.Equal(StorageVersion.VXAce, TypeMap.VXAce.Version);
        Assert.Equal(StorageVersion.VXAce, TypeMap.VXAce.Clone().Version);
        Assert.Equal(StorageVersion.MV, TypeMap.Mv.Clone().Version);
    }
}
