using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveChangeOpacityCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ChangeOpacity;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Opacity";
        else
            return "Change Opacity";
    }
    
    public int Opacity
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    // a new command is opaque in the editors
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, 255);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Opacity.ToString();
}