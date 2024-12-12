using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveEmptyCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Empty;
    public override string GetCommandName(StorageVersion version) => "Empty";
}