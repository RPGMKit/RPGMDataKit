using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

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

    public int ActorId { get; set; }
    public CheckType Check { get; set; }
    public object Param { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        ActorId = parameterProvider.GetParameter<int>(1);
        Check = (CheckType)parameterProvider.GetParameter<int>(2);
        Param = parameterProvider.GetParameter<object>(3);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);

        parameterProvider.SetParameter(1, ActorId);
        parameterProvider.SetParameter(2, (int)Check);
        parameterProvider.SetParameter(3, Param);
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
                var className = context.NameFormatter.FormatClass((int)Param);
                return context.Version == StorageVersion.MV
                    ? $"Class of {unitName} is {className}"
                    : $"{unitName} is {className}";
            case CheckType.Skill:
                var skillName = context.NameFormatter.FormatSkill((int)Param);
                return context.Version == StorageVersion.MV
                    ? $"{unitName} has learned {skillName}"
                    : $"{unitName} is {skillName} Learned";
            case CheckType.WeaponEquipped:
                var weaponName = context.NameFormatter.FormatWeapon((int)Param);
                return context.Version == StorageVersion.MV
                    ? $"{unitName} has equipped {weaponName}"
                    : $"{unitName} is {weaponName} Equipped";
            case CheckType.ArmorEquipped:
                var armorName = context.NameFormatter.FormatArmor((int)Param);
                return context.Version == StorageVersion.MV
                    ? $"{unitName} has equipped {armorName}"
                    : $"{unitName} is {armorName} Equipped";
            case CheckType.State:
                var stateName = context.NameFormatter.FormatState((int)Param);
                return context.Version == StorageVersion.MV
                    ? $"{unitName} is affected by {stateName}"
                    : $"{unitName} is {stateName} Inflicted";
            default:
                throw new Exception(
                    $"Unknown check type '{Check}' for '{GetCommandName(context.Version)}' actor command");
        }
    }
}