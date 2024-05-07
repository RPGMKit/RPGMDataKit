using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeClass : NamedCommand
    {
        public override int Code => CommandCodes.ChangeClass;
        public override string GetCommandName(StorageVersion version) => "Change Class";

        public int GetActor()
        {
            return GetParameter<int>(0);
        }

        public int GetClass()
        {
            return GetParameter<int>(1);
        }
        
        public bool GetUnk()
        {
            return GetParameter<bool>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return $"{context.NameFormatter.FormatActor(GetActor())}, {context.NameFormatter.FormatClass(GetClass())}";
        }
    }
}