using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ShopProcessingLineCommand : ShopProcessingCommand
{
    public ShopProcessingLineCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.ShopProcessingLine;
    public override string GetCommandName(StorageVersion version) => "               ";
}