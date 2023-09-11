using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class WaitCommand : NamedCommand
    {
        public override int Code => CommandCodes.Wait;
        public override string Name => "Wait";

        public int GetFrameCount()
        {
            return (int)GetParameter(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"{GetFrameCount()} frame(s)";
        }
    }
}
