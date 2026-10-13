using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ReplayBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.ReplayBgm;
    public override string GetCommandName(StorageVersion version) => "Replay BGM";
}