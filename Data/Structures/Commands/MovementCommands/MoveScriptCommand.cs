using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveScriptCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Script;
        public override string GetCommandName(StorageVersion version) => "Script";

        public string GetScript()
        {
            return GetParameter<string>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetScript();
        }
    }
}
