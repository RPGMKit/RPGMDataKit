using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveChangeSpeedCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ChangeSpeed;
    public override string GetCommandName(StorageVersion version) => "Speed";

    public enum SpeedType
    {
        Slowerx8 = 1,
        Slowerx4 = 2,
        Slowerx2 = 3,
        Normal = 4,
        Fasterx2 = 5,
        Fasterx4 = 6,
    }

    public SpeedType Speed
    {
        get => Get<SpeedType>(0);
        set => Set(0, value);
    }

    // a new command has the normal speed in the editors
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, SpeedType.Normal);
    }

    // the number, as the editors show it
    public override string GetParametersDescription(EntityContext context, DumpOptions options) => ((int)Speed).ToString();
}