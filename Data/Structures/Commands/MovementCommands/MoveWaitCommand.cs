using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveWaitCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Wait;
    public override string GetCommandName(StorageVersion version) => "Wait";

    public int Frames
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{Frames} frame(s)";
    }
}