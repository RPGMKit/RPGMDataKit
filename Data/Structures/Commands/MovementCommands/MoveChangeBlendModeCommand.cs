using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveChangeBlendModeCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ChangeBlendMode;
    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Blend Mode";
        else
            return "Change Blending";
    }
    
    public BlendMode BlendMode
    {
        get => Get<BlendMode>(0);
        set
        {
            if (value == BlendMode.Screen && Version != StorageVersion.MV)
                throw new Exception($"Unsupported blend mode {value}");

            Set(0, value);
        }
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        switch (BlendMode)
        {
            case BlendMode.Normal:
                return "Normal";
            case BlendMode.Add:
                return context.Version == StorageVersion.MV ? "Additive" : "Add";
            case BlendMode.Sub:
            //case BlendMode.Multiply:
                return context.Version == StorageVersion.MV ? "Multiply" : "Sub";
            case BlendMode.Screen:
                return context.Version == StorageVersion.MV ? "Screen" : $"Unknown ({BlendMode})";
            default:
                return $"Unknown ({BlendMode})";
        }
    }
}