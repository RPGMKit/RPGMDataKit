using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfActorCommand : IfCommand
    {
        protected const int CHECK_IN_PARTY = 0;
        protected const int CHECK_NAME = 1;
        protected const int CHECK_CLASS = 2;
        protected const int CHECK_SKILL = 3;
        protected const int CHECK_WEAPON_EQUIPPED = 4;
        protected const int CHECK_ARMOR_EQUIPPED = 5;
        protected const int CHECK_STATE = 6;

        public int GetUnitId()
        {
            return GetParameter<int>(1);
        }

        public int GetCheckType()
        {
            return GetParameter<int>(2);
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
                case CHECK_IN_PARTY:
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} is in the party"
                        : $"{unitName} is In the Party";
                case CHECK_NAME:
                    var name = GetCheckParameter<string>();
                    return context.Version == StorageVersion.MV
                        ? $"Name of {unitName} is {name}"
                        : $"{unitName} is Name '{name}' Applied";
                case CHECK_CLASS:
                    var className = context.NameFormatter.FormatClass(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"Class of {unitName} is {className}"
                        : $"{unitName} is {className}";
                case CHECK_SKILL:
                    var skillName = context.NameFormatter.FormatSkill(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} has learned {skillName}"
                        : $"{unitName} is {skillName} Learned";
                case CHECK_WEAPON_EQUIPPED:
                    var weaponName = context.NameFormatter.FormatWeapon(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} has equipped {weaponName}"
                        : $"{unitName} is {weaponName} Equipped";
                case CHECK_ARMOR_EQUIPPED:
                    var armorName = context.NameFormatter.FormatArmor(GetCheckParameter<int>());
                    return context.Version == StorageVersion.MV
                        ? $"{unitName} has equipped {armorName}"
                        : $"{unitName} is {armorName} Equipped";
                case CHECK_STATE:
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
