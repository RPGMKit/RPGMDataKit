using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveWaitCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Wait;
    public override string GetCommandName(StorageVersion version) => "Wait";

    public int Frames
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    // a new command waits for 60 frames in the editors
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, 60);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{Frames} frame(s)";
    }
}