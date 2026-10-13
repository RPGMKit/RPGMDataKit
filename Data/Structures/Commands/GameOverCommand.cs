using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class GameOverCommand : NamedCommand
{
    public override int Code => CommandCodes.GameOver;
    public override string GetCommandName(StorageVersion version) => "Game Over";
}