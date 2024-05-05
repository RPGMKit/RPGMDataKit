using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveScriptCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Script;
        public override string Name => "Script";

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
