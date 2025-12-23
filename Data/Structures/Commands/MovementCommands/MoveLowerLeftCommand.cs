using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveLowerLeftCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveLowerLeft;
    public override string GetCommandName(StorageVersion version) => "Move Lower Left";
}