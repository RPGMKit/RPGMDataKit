using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShakeScreenCommand : NamedCommand
    {
        public override int Code => CommandCodes.ShakeScreen;
        public override string Name => "Shake Screen";

        public int GetPower()
        {
            return (int)GetParameter(0);
        }

        public int GetSpeed()
        {
            return (int)GetParameter(1);
        }

        public int GetDuration()
        {
            return (int)GetParameter(2);
        }

        public bool GetWaitForCompletion()
        {
            return (bool)GetParameter(3);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"{GetPower()}, {GetSpeed()}, {GetDuration()} frame(s)";
            if (GetWaitForCompletion())
                str += " (Wait)";

            return str;
        }
    }
}