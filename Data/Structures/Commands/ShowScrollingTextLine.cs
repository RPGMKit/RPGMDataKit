using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowScrollingTextLine : NamedCommand
    {
        public ShowScrollingTextLine()
        {
            IsSecondary = true;
        }

        public override string GetCommandName(StorageVersion version) => "    ";
        
        public override int Code => CommandCodes.ShowScrollingTextLine;

        public string GetText()
        {
            return GetParameter<string>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetText();
        }
    }
}
