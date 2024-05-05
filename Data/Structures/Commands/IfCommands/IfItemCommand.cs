using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfItemCommand : IfCommand
    {
        public int GetItem()
        {
            return GetParameter<int>(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"Party has item #{GetItem()}";
        }
    }
}
