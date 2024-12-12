using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ReplayBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.ReplayBgm;
    public override string GetCommandName(StorageVersion version) => "Replay BGM";
}