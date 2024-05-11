using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeEquipmentCommand : NamedCommand
    {
        private const int EquipmentTypeWeapon = 0;
        private const int EquipmentTypeShield = 1;
        private const int EquipmentTypeHead = 2;
        private const int EquipmentTypeBody = 3;
        private const int EquipmentTypeAccessory = 4;

        private static readonly Dictionary<int, string> LegacyEquipmentTypeToName = new()
        {
            [EquipmentTypeWeapon] = "Weapon",
            [EquipmentTypeShield] = "Shield",
            [EquipmentTypeHead] = "Head",
            [EquipmentTypeBody] = "Body",
            [EquipmentTypeAccessory] = "Accessory",
        };

        public override int Code => CommandCodes.ChangeEquipment;
        public override string GetCommandName(StorageVersion version) => "Change Equipment";

        public int GetActor()
        {
            return GetParameter<int>(0);
        }

        public int GetEquipmentType()
        {
            return GetParameter<int>(1);
        }

        public int GetEquipmentItem()
        {
            return GetParameter<int>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = $"{context.NameFormatter.FormatActor(GetActor())}, ";

            var type = GetEquipmentType();
            if (context.Version == StorageVersion.MV)
            {
                var typeName = context.Storage.System.Get().EquipmentTypes.Get(type);
                if (string.IsNullOrEmpty(typeName))
                    throw new Exception($"Unknown equipment type id {type} for change equipment command");

                str += $"{typeName} = ";
            }
            else
            {
                if (!LegacyEquipmentTypeToName.ContainsKey(type))
                    throw new Exception($"Unknown equipment type id {type} for change equipment command");

                str += $"{LegacyEquipmentTypeToName[type]} = ";
            }

            if (GetEquipmentItem() == 0)
            {
                str += context.Version == StorageVersion.MV ? "None" : "(None)";
                return str;
            }

            if (context.Version == StorageVersion.MV)
                str += context.NameFormatter.FormatArmor(GetEquipmentItem());
            else
            {
                switch (GetEquipmentType())
                {
                    case EquipmentTypeWeapon:
                        str += context.NameFormatter.FormatWeapon(GetEquipmentItem());
                        break;
                    default:
                        str += context.NameFormatter.FormatArmor(GetEquipmentItem());
                        break;
                }
            }

            return str;
        }
    }
}
