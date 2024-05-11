using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveBlendModeCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.BlendMode;
        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Blend Mode";
            else
                return "Change Blending";
        }

        public BlendMode GetBlendMode()
        {
            return (BlendMode)GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            switch (GetBlendMode())
            {
                case BlendMode.Normal:
                    return "Normal";
                case BlendMode.Add:
                    return context.Version == StorageVersion.MV ? "Additive" : "Add";
                case BlendMode.Sub:
                    return "Sub";
                default:
                    return $"Unknown ({GetBlendMode()})";
            }
        }
    }
}
