using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveJumpCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Jump;
    public override string GetCommandName(StorageVersion version) => "Jump";
    
    public int XDelta
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public int YDelta
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{XDelta:+#;-#;0}, {YDelta:+#;-#;0}";
    }
}