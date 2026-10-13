using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveChangeSpeedCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ChangeSpeed;
    public override string GetCommandName(StorageVersion version) => "Speed";

    public int Speed
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Speed.ToString();
}