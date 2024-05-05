using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ScriptCommand : NamedCommand
    {
        public override int Code => CommandCodes.Script;
        public override string GetCommandName(StorageVersion version) => "Script";

        public string GetScript()
        {
            return GetParameter<string>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetScript();
        }
    }
}
