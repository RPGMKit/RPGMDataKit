using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveTransparentOffCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TransparentOff;
    public override string GetCommandName(StorageVersion version) => "Transparent OFF";
}