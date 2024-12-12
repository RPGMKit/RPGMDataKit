using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class AbortBattleCommand : NamedCommand
{
    public override int Code => CommandCodes.AbortBattle;
    public override string GetCommandName(StorageVersion version) => "Abort Battle";
}