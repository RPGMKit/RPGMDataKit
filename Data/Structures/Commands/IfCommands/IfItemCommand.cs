using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfItemCommand : IfCommand
{
    public int ItemId { get; set; }
    
    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        ItemId = parameterProvider.GetParameter<int>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, ItemId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var item = context.NameFormatter.FormatItem(ItemId);
            
        return context.Version == StorageVersion.MV ? $"Party has {item}" : $"{item} in Inventory";
    }
}