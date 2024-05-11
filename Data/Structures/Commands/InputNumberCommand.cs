using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class InputNumberCommand : NamedCommand
    {
        public int GetVariable()
        {
            return GetParameter<int>(0);
        }

        public int GetDigitsCount()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.InputNumber;
        public override string GetCommandName(StorageVersion version) => "Input Number";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var variableName = context.NameFormatter.FormatVariable(GetVariable(), new()
            {
                BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
            });
            
            var str = $"{variableName}, {GetDigitsCount()} digits";

            return str;
        }
    }
}
