using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetWeatherEffectCommand : NamedCommand
    {
        protected const string TypeNone = "none";

        public override int Code => CommandCodes.SetWeatherEffect;

        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Set Weather Effect";
            else
                return "Set Weather Effects";
        }

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

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = "";
            if (GetEffectType() == TypeNone)
                str = "None";
            else
                str = GetEffectType().UcFirst() + $", {GetPower()}";

            str += context.Version == StorageVersion.MV ? $", {GetDuration()} frame(s)" : $", @{GetDuration()}";
            if (GetWaitForCompletion())
                str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

            return str;
        }
    }
}
