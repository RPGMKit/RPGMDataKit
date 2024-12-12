using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfWeaponCommand : IfCommand
{
    public int WeaponId { get; set; }
    public bool IncludeEquipment { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        WeaponId = parameterProvider.GetParameter<int>(1);
        IncludeEquipment = parameterProvider.GetParameter<bool>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, WeaponId);
        parameterProvider.SetParameter(2, IncludeEquipment);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var weapon = context.NameFormatter.FormatWeapon(WeaponId);
        var str = context.Version == StorageVersion.MV ? $"Party has {weapon}" : $"{weapon} in Inventory";
        if (IncludeEquipment)
            str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

        return str;
    }
}