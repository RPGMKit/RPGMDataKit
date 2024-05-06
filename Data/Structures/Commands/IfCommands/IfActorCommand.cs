using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;

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

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var unitName = storage.NameFormatter.FormatActor(GetUnitId());

            switch (GetCheckType())
            {
                case CHECK_IN_PARTY:
                    return storage.Version == StorageVersion.VXAce
                        ? $"{unitName} is In the Party"
                        : $"{unitName} is in the party";
                case CHECK_NAME:
                    var name = GetCheckParameter<string>();
                    return storage.Version == StorageVersion.VXAce
                        ? $"{unitName} is Name '{name}' Applied"
                        : $"{unitName} name is '{name}'";
                case CHECK_CLASS:
                    var className = storage.NameFormatter.FormatClass(GetCheckParameter<int>());
                    return storage.Version == StorageVersion.VXAce
                        ? $"{unitName} is {className}"
                        : $"Class of {unitName} is {className}";
                case CHECK_SKILL:
                    var skillName = storage.NameFormatter.FormatSkill(GetCheckParameter<int>());
                    return storage.Version == StorageVersion.VXAce
                        ? $"{unitName} is {skillName} Learned"
                        : $"{unitName} has learned {skillName}";
                case CHECK_WEAPON_EQUIPPED:
                    var weaponName = storage.NameFormatter.FormatWeapon(GetCheckParameter<int>());
                    return storage.Version == StorageVersion.VXAce
                        ? $"{unitName} is {weaponName} Equipped"
                        : $"{unitName} has equipped {weaponName}";
                case CHECK_ARMOR_EQUIPPED:
                    var armorName = storage.NameFormatter.FormatArmor(GetCheckParameter<int>());
                    return storage.Version == StorageVersion.VXAce
                        ? $"{unitName} is {armorName} Equipped"
                        : $"{unitName} has equipped {armorName}";
                case CHECK_STATE:
                    var stateName = storage.NameFormatter.FormatState(GetCheckParameter<int>());
                    return storage.Version == StorageVersion.VXAce
                        ? $"{unitName} is {stateName} Inflicted"
                        : $"{unitName} is affected by {stateName}";
                default:
                    throw new Exception(
                        $"Unknown check type '{GetCheckType()}' for '{GetCommandName(storage.Version)}' actor command");
            }
        }
    }
}
