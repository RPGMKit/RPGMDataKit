using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShakeScreenCommand : NamedCommand
    {
        public override int Code => CommandCodes.ShakeScreen;
        public override string GetCommandName(StorageVersion version) => "Shake Screen";

        public int GetPower()
        {
            return GetParameter<int>(0);
        }

        public int GetSpeed()
        {
            return GetParameter<int>(1);
        }

        public int GetDuration()
        {
            return GetParameter<int>(2);
        }

        public bool GetWaitForCompletion()
        {
            return GetParameter<bool>(3);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            if (context.Version == StorageVersion.VXAce)
            {
                var str = $"{GetPower()}, {GetSpeed()}, @{GetDuration()}";
                if (GetWaitForCompletion())
                    str += ", Wait";

                return str;
            }
            else
            {
                var str = $"{GetPower()}, {GetSpeed()}, {GetDuration()} frame(s)";
                if (GetWaitForCompletion())
                    str += " (Wait)";

                return str;
            }
        }
    }
}