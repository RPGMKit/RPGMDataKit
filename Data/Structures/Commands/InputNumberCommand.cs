using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class InputNumberCommand : NamedCommand
    {
        public int GetVariable()
        {
            return (int)GetParameter(0);
        }

        public int GetDigitsCount()
        {
            return (int)GetParameter(1);
        }

        public override int Code => CommandCodes.InputNumber;
        public override string Name => "Input number";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetVariable()}";
            var variableName = storage.GetVariable(GetVariable());
            if (!string.IsNullOrEmpty(variableName))
            {
                str += $" {variableName}";
            }

            str += $", {GetDigitsCount()} digits";

            return str;
        }
    }
}
