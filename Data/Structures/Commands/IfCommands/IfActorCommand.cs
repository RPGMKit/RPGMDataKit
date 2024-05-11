using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
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

        public int GetUnitId()
        {
            return GetParameter<int>(1);
        }

        public CheckType GetCheckType()
        {
            return (CheckType)GetParameter<int>(2);
        }

        public T? GetCheckParameter<T>()
        {
            return GetParameter<T>(3);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var unitName = context.NameFormatter.FormatActor(GetUnitId());

            switch (GetCheckType())
            {
                case CheckType.InParty:
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} is in the party"
                        : $"{unitName} is In the Party";
                case CheckType.Name:
                    var name = GetCheckParameter<string>();
                    return context.Version == StorageVersion.MV
                        ? $"Name of {unitName} is {name}"
                        : $"{unitName} is Name '{name}' Applied";
                case CheckType.Class:
                    var className = context.NameFormatter.FormatClass(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"Class of {unitName} is {className}"
                        : $"{unitName} is {className}";
                case CheckType.Skill:
                    var skillName = context.NameFormatter.FormatSkill(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} has learned {skillName}"
                        : $"{unitName} is {skillName} Learned";
                case CheckType.WeaponEquipped:
                    var weaponName = context.NameFormatter.FormatWeapon(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} has equipped {weaponName}"
                        : $"{unitName} is {weaponName} Equipped";
                case CheckType.ArmorEquipped:
                    var armorName = context.NameFormatter.FormatArmor(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} has equipped {armorName}"
                        : $"{unitName} is {armorName} Equipped";
                case CheckType.State:
                    var stateName = context.NameFormatter.FormatState(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} is affected by {stateName}"
                        : $"{unitName} is {stateName} Inflicted";
                default:
                    throw new Exception(
                        $"Unknown check type '{GetCheckType()}' for '{GetCommandName(context.Version)}' actor command");
            }
        }
    }
}
