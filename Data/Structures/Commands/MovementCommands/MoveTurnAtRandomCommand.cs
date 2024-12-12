using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveTurnAtRandomCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TurnAtRandom;
    public override string GetCommandName(StorageVersion version) => "Turn at Random";
}