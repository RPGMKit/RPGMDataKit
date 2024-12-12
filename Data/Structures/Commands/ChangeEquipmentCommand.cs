using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeEquipmentCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeEquipment;
    public override string GetCommandName(StorageVersion version) => "Change Equipment";

    public enum LegacyEquipmentType
    {
        Weapon = 0,
        Shield = 1,
        Head = 2,
        Body = 3,
        Accessory = 4,
    }
        
    public int ActorId { get; set; }
    public int TypeId { get; set; }
    public int ItemId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ActorId = parameterProvider.GetParameter<int>(0);
        TypeId = parameterProvider.GetParameter<int>(1);
        ItemId = parameterProvider.GetParameter<int>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ActorId);
        parameterProvider.SetParameter(1, TypeId);
        parameterProvider.SetParameter(2, ItemId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = $"{context.NameFormatter.FormatActor(ActorId)}, ";

        if (context.Version == StorageVersion.MV)
        {
            var typeName = context.Storage.System.Get().EquipmentTypes.Get(TypeId);
            if (string.IsNullOrEmpty(typeName))
                throw new Exception($"Unknown equipment type {TypeId} for change equipment command");

            str += $"{typeName} = ";
        }
        else
            str += $"{TypeId} = ";

        if (ItemId == 0)
        {
            str += context.Version == StorageVersion.MV ? "None" : "(None)";
            return str;
        }

        if (context.Version == StorageVersion.MV)
            str += context.NameFormatter.FormatArmor(ItemId);
        else
        {
            str += (LegacyEquipmentType)TypeId switch
            {
                LegacyEquipmentType.Weapon => context.NameFormatter.FormatWeapon(ItemId),
                _ => context.NameFormatter.FormatArmor(ItemId)
            };
        }

        return str;
    }
}