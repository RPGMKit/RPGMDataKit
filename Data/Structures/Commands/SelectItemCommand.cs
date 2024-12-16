using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class SelectItemCommand : NamedCommand
{
    public override int Code => CommandCodes.SelectItem;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Select Item";
        else
            return "Select Key Item";
    }
    
    public enum KeyItemType
    {
        Regular = 1,
        Key = 2,
        HiddenA = 3,
        HiddenB = 4,
    }

    private static readonly Dictionary<KeyItemType, string> KeyItemTypeNames = new()
    {
        [KeyItemType.Regular] = "Regular Item",
        [KeyItemType.Key] = "Key Item",
        [KeyItemType.HiddenA] = "Hidden Item A",
        [KeyItemType.HiddenB] = "Hidden Item B",
    };

    public int VariableId { get; set; }
    public KeyItemType Type { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        VariableId = parameterProvider.GetParameter<int>(0);
        Type = parameterProvider.GetParameter<KeyItemType>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, VariableId);
        parameterProvider.SetParameter(1, Type);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatVariable(VariableId, new()
        {
            BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
        });

        if (context.Version == StorageVersion.MV)
        {
            if (!KeyItemTypeNames.TryGetValue(Type, out var itemType))
                throw new Exception($"Unknown item type for select item command: {Type}");

            str += $", {itemType}";
        }

        return str;
    }
}