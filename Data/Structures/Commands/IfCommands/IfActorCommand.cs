using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfActorCommand : IfCommand
{
    public enum CheckType
    {
        InParty = 0,
        Name = 1,
        Class = 2,
        Skill = 3,
        WeaponEquipped = 4,
        ArmorEquipped = 5,
        State = 6,
    }

    [EditorDefault(1)]
    public int ActorId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public CheckType Check
    {
        get => Get<CheckType>(2);
        set
        {
            Set(2, value);
            if (value == CheckType.InParty && !IsDetached)
                DropParam();
        }
    }

    // "in the party" has no fourth parameter in the files of the editors; the other kinds have what they check
    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        base.OnMadeInCodeBound(version);
        if (Check == CheckType.InParty)
            DropParam();
    }

    private void DropParam()
    {
        if (Raw.Parameters is { Count: > 3 } parameters)
            parameters.RemoveRange(3, parameters.Count - 3);
    }

    public object Param
    {
        get => Get<object>(3);
        set => Set(3, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var unitName = context.NameFormatter.FormatActor(ActorId);

        switch (Check)
        {
            case CheckType.InParty:
                return context.Version == StorageVersion.MV
                    ? $"{unitName} is in the party"
                    : $"{unitName} is In the Party";
            case CheckType.Name:
                var name = (string)Param;
                return context.Version == StorageVersion.MV
                    ? $"Name of {unitName} is {name}"
                    : $"{unitName} is Name '{name}' Applied";
            case CheckType.Class:
                var className = context.NameFormatter.FormatClass(Convert.ToInt32(Param));
                return context.Version == StorageVersion.MV
                    ? $"Class of {unitName} is {className}"
                    : $"{unitName} is {className}";
            case CheckType.Skill:
                var skillName = context.NameFormatter.FormatSkill(Convert.ToInt32(Param));
                return context.Version == StorageVersion.MV
                    ? $"{unitName} has learned {skillName}"
                    : $"{unitName} is {skillName} Learned";
            case CheckType.WeaponEquipped:
                var weaponName = context.NameFormatter.FormatWeapon(Convert.ToInt32(Param));
                return context.Version == StorageVersion.MV
                    ? $"{unitName} has equipped {weaponName}"
                    : $"{unitName} is {weaponName} Equipped";
            case CheckType.ArmorEquipped:
                var armorName = context.NameFormatter.FormatArmor(Convert.ToInt32(Param));
                return context.Version == StorageVersion.MV
                    ? $"{unitName} has equipped {armorName}"
                    : $"{unitName} is {armorName} Equipped";
            case CheckType.State:
                var stateName = context.NameFormatter.FormatState(Convert.ToInt32(Param));
                return context.Version == StorageVersion.MV
                    ? $"{unitName} is affected by {stateName}"
                    : $"{unitName} is {stateName} Inflicted";
            default:
                throw new Exception(
                    $"Unknown check type '{Check}' for '{GetCommandName(context.Version)}' actor command");
        }
    }
}