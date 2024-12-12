using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfArmorCommand : IfCommand
{
    public int ArmorId { get; set; }
    public bool IncludeEquipment { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        ArmorId = parameterProvider.GetParameter<int>(1);
        IncludeEquipment = parameterProvider.GetParameter<bool>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, ArmorId);
        parameterProvider.SetParameter(2, IncludeEquipment);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var armor = context.NameFormatter.FormatArmor(ArmorId);
        var str = context.Version == StorageVersion.MV ? $"Party has {armor}" : $"{armor} in Inventory";
        if (IncludeEquipment)
            str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

        return str;
    }
}