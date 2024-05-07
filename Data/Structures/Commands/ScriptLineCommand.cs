using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ScriptLineCommand : NamedCommand
    {
        public ScriptLineCommand()
        {
            IsSecondary = true;
        }

        public override string GetCommandName(StorageVersion version) => "      ";
        
        public override int Code => CommandCodes.ScriptLineCommand;

        public string GetText()
        {
            return GetParameter<string>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetText();
        }
    }
}
