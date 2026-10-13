using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class SaveBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.SaveBgm;
    public override string GetCommandName(StorageVersion version) => "Save BGM";
}