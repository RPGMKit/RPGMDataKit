using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveRandomCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveRandom;
    public override string GetCommandName(StorageVersion version) => "Move at Random";
}