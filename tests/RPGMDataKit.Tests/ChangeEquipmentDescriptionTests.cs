using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// How Change Equipment reads in the text dump. VX Ace counts the slots from 0 and the item of the first one is a weapon; MV counts them from 1
/// (the type of the equipment, the first one is the weapon), and for the other slots the item is an armor.
/// </summary>
public sealed class ChangeEquipmentDescriptionTests
{
    private static Storage Open(Version v)
    {
        var project = Path.Combine(AppContext.BaseDirectory, "Projects", v.Name);

        return v.Name == "MV" ? new MVStorage(project, "data") : new VXAceStorage(project, "Data");
    }

    private static string Describe(Version v, int typeId, int itemId)
    {
        var command = new ChangeEquipmentCommand { ActorId = 1, TypeId = typeId, ItemId = itemId };

        return command.GetParametersDescription(new EntityContext(Open(v), extendedFormat: true), new DumpOptions { ExtendedFormat = true });
    }

    // ---- an actor with two weapons: the feature "Slot Type" with the value 1, of the actor or of its class ----

    private static void GiveDualWield(Storage storage, IHasFeatures owner)
    {
        var trait = new Data.MV.Classes.Trait { Code = FeatureCodes.SlotType, DataId = 1, Value = 1 };
        var feature = new Data.VXAce.Classes.Feature { Code = FeatureCodes.SlotType, DataId = 1, Value = 0 };
        switch (owner)
        {
            case Data.MV.Classes.Actor actor:
                actor.Traits.Add(trait);
                break;
            case Data.MV.Classes.Class @class:
                @class.Traits.Add(trait);
                break;
            case Data.VXAce.Classes.BaseItem item:
                item.Features.Add(feature);
                break;
            default:
                throw new NotSupportedException(owner.GetType().Name);
        }
    }

    private static string Describe(Storage storage, int actorId, int typeId, int itemId) =>
        new ChangeEquipmentCommand { ActorId = actorId, TypeId = typeId, ItemId = itemId }
            .GetParametersDescription(new EntityContext(storage, extendedFormat: true), new DumpOptions { ExtendedFormat = true });

    [Fact]
    public void VxAceCallsTheTwoSlotsOfAnActorWithTwoWeaponsWeapon1AndWeapon2()
    {
        var storage = Open(Version.VXAce);
        GiveDualWield(storage, storage.Actors.Get(1)!);

        Assert.Contains(", Weapon 1 = ", Describe(storage, 1, 0, 1));
        Assert.Contains(", Weapon 2 = ", Describe(storage, 1, 1, 1));
        Assert.Contains(":W:", Describe(storage, 1, 0, 1));
        Assert.Contains(":W:", Describe(storage, 1, 1, 1)); // the item of the second slot is a weapon
        Assert.Contains(", Head = ", Describe(storage, 1, 2, 1));
        Assert.Contains(":A:", Describe(storage, 1, 2, 1));
    }

    [Fact]
    public void MvCallsBothSlotsOfAnActorWithTwoWeaponsWeapon()
    {
        var storage = Open(Version.Mv);
        GiveDualWield(storage, storage.Actors.Get(1)!);
        var weapon = storage.System.Get().EquipmentTypes.Get(1);

        Assert.Contains($", {weapon} = ", Describe(storage, 1, 1, 1));
        Assert.Contains($", {weapon} = ", Describe(storage, 1, 2, 1)); // the second slot
        Assert.Contains(":W:", Describe(storage, 1, 2, 1));
        Assert.Contains(":A:", Describe(storage, 1, 3, 1)); // the third slot is the head
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AnActorWithoutTheFeatureKeepsTheShield(Version v)
    {
        var storage = Open(v);
        GiveDualWield(storage, storage.Actors.Get(1)!);
        var other = storage.Actors.Get(2)!;
        Assert.DoesNotContain(other.Features, f => f.Code == FeatureCodes.SlotType);
        Assert.DoesNotContain(storage.Classes.Get(other.ClassId)!.Features, f => f.Code == FeatureCodes.SlotType);

        var shield = Describe(storage, 2, v.Name == "MV" ? 2 : 1, 1);

        Assert.Contains(v.Name == "MV" ? storage.System.Get().EquipmentTypes.Get(2) : "Shield", shield);
        Assert.Contains(":A:", shield);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheFeatureOfTheClassCountsToo(Version v)
    {
        var storage = Open(v);
        var actor = storage.Actors.Get(2)!;
        GiveDualWield(storage, storage.Classes.Get(actor.ClassId)!);

        var secondSlot = Describe(storage, 2, v.Name == "MV" ? 2 : 1, 1);

        Assert.Contains(":W:", secondSlot);
    }

    [Fact]
    public void MvTakesTheItemOfTheWeaponSlotFromTheWeapons()
    {
        Assert.Contains(":W:", Describe(Version.Mv, 1, 1));
        Assert.DoesNotContain(":A:", Describe(Version.Mv, 1, 1));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public void MvTakesTheItemOfTheOtherSlotsFromTheArmors(int typeId)
    {
        Assert.Contains(":A:", Describe(Version.Mv, typeId, 1));
        Assert.DoesNotContain(":W:", Describe(Version.Mv, typeId, 1));
    }

    [Fact]
    public void MvNamesTheSlotWithTheTypeOfTheSystem()
    {
        var typeName = Open(Version.Mv).System.Get().EquipmentTypes.Get(2);

        Assert.Contains($", {typeName} = ", Describe(Version.Mv, 2, 1));
    }

    [Theory]
    [InlineData(0, "Weapon", ":W:")]
    [InlineData(1, "Shield", ":A:")]
    [InlineData(2, "Head", ":A:")]
    [InlineData(3, "Body", ":A:")]
    [InlineData(4, "Accessory", ":A:")]
    public void VxAceNamesTheSlotAndTakesTheItemFromTheWeaponsForTheFirstOne(int typeId, string slot, string kind)
    {
        var text = Describe(Version.VXAce, typeId, 1);

        Assert.Contains($", {slot} = ", text);
        Assert.Contains(kind, text);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void NoItemIsNone(Version v)
    {
        Assert.EndsWith(v.Name == "MV" ? " = None" : " = (None)", Describe(v, v.Name == "MV" ? 2 : 3, 0));
    }

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });
}
