using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShopProcessingLineCommand : ShopProcessingCommand
    {
        public ShopProcessingLineCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.ShopProcessingLine;
        public override string Name => "               ";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"#{GetId()}";
        }
    }
}