using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfItemCommand : IfCommand
{
    public int ItemId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var item = context.NameFormatter.FormatItem(ItemId);
            
        return context.Version == StorageVersion.MV ? $"Party has {item}" : $"{item} in Inventory";
    }
}