using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetWeatherEffectCommand : NamedCommand
    {
        protected const string TYPE_NONE = "none";

        public override int Code => CommandCodes.SetWeatherEffect;
        public override string Name => "Set Weather Effect";

        public string GetEffectType()
        {
            return GetParameter<string>(0);
        }

        public int GetPower()
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

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            if (GetEffectType() == TYPE_NONE)
                str = "None";
            else
                str = GetEffectType().UcFirst() + $", {GetPower()}";

            str += $", {GetDuration()} frame(s)";
            if (GetWaitForCompletion())
                str += " (Wait)";

            return str;
        }
    }
}
