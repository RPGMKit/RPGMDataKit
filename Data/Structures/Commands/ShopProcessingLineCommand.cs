using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShopProcessingLineCommand : ShopProcessingCommand
    {
        public ShopProcessingLineCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.ShopProcessingLine;
        public override string GetCommandName(StorageVersion version) => "               ";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return $"#{GetId()}";
        }
    }
}