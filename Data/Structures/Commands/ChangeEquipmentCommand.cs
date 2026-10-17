using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures.Attributes;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeEquipmentCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeEquipment;
    public override string GetCommandName(StorageVersion version) => "Change Equipment";

    private const int WeaponTypeIdOfMv = 1;

    public enum LegacyEquipmentType
    {
        Weapon = 0,
        Shield = 1,
        Head = 2,
        Body = 3,
        Accessory = 4,
    }
        
    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    /// <summary>
    /// The raw number of the slot, which depends on the maker: VX Ace counts the slots from 0, MV from 1 (it is the type of the equipment there).
    /// Use <see cref="SlotIndex"/> for a number that means the same in both.
    /// </summary>
    [RawValue]
    public int TypeId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    private bool IsInMv => !IsDetached && Version == StorageVersion.MV;

    /// <summary>
    /// The slot of the actor counted from 0, the same for both makers (the raw number of MV is one more, see <see cref="TypeId"/>). With two
    /// weapons the slot 1 is the second weapon. Can be set before the command is put into a list, it then gets the number of the maker of the list.
    /// </summary>
    public int SlotIndex
    {
        get => IsInMv ? Get<int>(1) - 1 : Get<int>(1);
        set => Set(1, IsInMv ? value + 1 : value);
    }

    protected override void OnBinding(StorageVersion version)
    {
        // until the command has a maker the parameter holds the slot counted from 0 (the number of VX Ace); a slot that was not told is the first one
        if (version == StorageVersion.MV)
            Set(1, (IsParameterSet(1) ? Get<int>(1) : 0) + 1);
    }

    public int ItemId
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    // the names of the slots in the editor of VX Ace; an actor with two weapons has "Weapon 1" and "Weapon 2" in place of Weapon and Shield
    private static readonly Dictionary<LegacyEquipmentType, string> LegacySlotNames = new()
    {
        [LegacyEquipmentType.Weapon] = "Weapon",
        [LegacyEquipmentType.Shield] = "Shield",
        [LegacyEquipmentType.Head] = "Head",
        [LegacyEquipmentType.Body] = "Body",
        [LegacyEquipmentType.Accessory] = "Accessory",
    };
    private const int DualWeaponSlotId = 1;

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var mv = context.Version == StorageVersion.MV;
        var dualWield = IsDualWield(context, ActorId);
        var str = $"{context.NameFormatter.FormatActor(ActorId)}, ";

        // the type of the equipment that the slot holds: MV counts the slots from 1, the second one is a weapon too for an actor with two weapons
        var equipmentType = mv && dualWield && TypeId == DualWeaponSlotId + 1 ? WeaponTypeIdOfMv : TypeId;
        if (mv)
        {
            var typeName = context.Storage.System.Get().EquipmentTypes.Get(equipmentType);
            if (string.IsNullOrEmpty(typeName))
                throw new Exception($"Unknown equipment type {TypeId} for change equipment command");

            str += $"{typeName} = ";
        }
        else
        {
            str += VxSlotName(TypeId, dualWield) + " = ";
        }

        if (ItemId == 0)
        {
            str += mv ? "None" : "(None)";
            return str;
        }

        // the weapon slots take the item from the weapons, the others from the armors (the engines choose by the slots of the actor)
        var isWeapon = mv
            ? equipmentType == WeaponTypeIdOfMv
            : (LegacyEquipmentType)TypeId == LegacyEquipmentType.Weapon || (dualWield && TypeId == DualWeaponSlotId);
        str += isWeapon ? context.NameFormatter.FormatWeapon(ItemId) : context.NameFormatter.FormatArmor(ItemId);

        return str;
    }

    private static string VxSlotName(int slotId, bool dualWield)
    {
        if (dualWield && slotId is 0 or DualWeaponSlotId)
            return $"Weapon {slotId + 1}";

        return LegacySlotNames.TryGetValue((LegacyEquipmentType)slotId, out var name) ? name : slotId.ToString();
    }

    /// <summary>
    /// The actor has two weapons: the feature "Slot Type" (code 55) with the value 1 of the actor or of its class, the way the editors show the slots.
    /// (The engines also take the equipment and the states of the actor into account, which the data does not know.)
    /// </summary>
    private static bool IsDualWield(EntityContext context, int actorId)
    {
        var actor = context.Storage.Actors.Get(actorId);
        if (actor == null)
            return false;

        var slotType = SlotType(actor);
        var @class = context.Storage.Classes.Get(actor.ClassId);
        if (@class != null)
            slotType = Math.Max(slotType, SlotType(@class));

        return slotType == 1;
    }

    // The engines take the maximum of the "Slot Type" values and compare it with 1 (dual wield), which is roundabout: the editors only offer 0
    // (normal, only in MV) and 1 (dual wield, both VXAce and MV), so "the actor or its class has a feature with FeatureCodes.SlotType and DataId == 1" is enough in both
    // engines. The maximum is kept to do what the engines do, also for a value that is not 0 or 1 and was put into the data by hand.
    private static int SlotType(IHasFeatures owner) =>
        owner.Features.Where(f => f.Code == FeatureCodes.SlotType).Select(f => f.DataId).DefaultIfEmpty(0).Max();
}