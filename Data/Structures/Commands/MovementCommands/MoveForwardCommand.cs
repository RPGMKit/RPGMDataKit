using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveForwardCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveForward;
    public override string GetCommandName(StorageVersion version) => "1 Step Forward";
}