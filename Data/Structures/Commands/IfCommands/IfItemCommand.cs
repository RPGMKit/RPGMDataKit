using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfItemCommand : IfCommand
    {
        public int GetItem()
        {
            return GetParameter<int>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return $"Party has item #{GetItem()}";
        }
    }
}
