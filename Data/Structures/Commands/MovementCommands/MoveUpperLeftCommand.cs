using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveUpperLeftCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.UpperLeft;
    public override string GetCommandName(StorageVersion version) => "Move Upper Left";
}