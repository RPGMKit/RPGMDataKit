using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class AbortBattleCommand : NamedCommand
{
    public override int Code => CommandCodes.AbortBattle;
    public override string GetCommandName(StorageVersion version) => "Abort Battle";
}