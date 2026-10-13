using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveChangeFrequencyCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ChangeFrequency;
    public override string GetCommandName(StorageVersion version) => "Frequency";

    public int Frequency
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Frequency.ToString();
}
