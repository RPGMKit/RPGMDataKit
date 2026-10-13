using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public abstract class AbstractMoveCommand : NamedCommand
{
    protected override string GetStringPrefix(StorageVersion version)
    {
        return version == StorageVersion.MV ? "◇" : "$>";
    }
}