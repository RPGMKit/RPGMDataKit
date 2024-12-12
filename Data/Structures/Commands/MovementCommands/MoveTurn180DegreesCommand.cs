using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveTurn180DegreesCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Turn180Degrees;
    public override string GetCommandName(StorageVersion version) => "Turn 180°";
}