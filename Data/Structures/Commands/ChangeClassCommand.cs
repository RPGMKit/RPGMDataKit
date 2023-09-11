using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeClass : NamedCommand
    {
        public override int Code => CommandCodes.ChangeClass;
        public override string Name => "Change Class";

        public int GetActor()
        {
            return (int)GetParameter(0);
        }

        public int GetClass()
        {
            return (int)GetParameter(1);
        }
        
        public bool GetUnk()
        {
            return (bool)GetParameter(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetActor()}, #{GetClass()}";

            return str;
        }
    }
}