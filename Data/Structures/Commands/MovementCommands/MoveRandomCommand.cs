using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveRandomCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveRandom;
    public override string GetCommandName(StorageVersion version) => "Move at Random";
}