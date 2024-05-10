using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveBlendModeCommand : AbstractMoveCommand
    {
        protected const int BLEND_NORMAL = 0;
        protected const int BLEND_ADDITIVE = 1;
        protected const int BLEND_SUB = 2;

        public override int Code => MovementRouteCommandCodes.BlendMode;
        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Blend Mode";
            else
                return "Change Blending";
        }

        public int GetBlendMode()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            switch (GetBlendMode())
            {
                case BLEND_NORMAL:
                    return "Normal";
                case BLEND_ADDITIVE:
                    return context.Version == StorageVersion.MV ? "Additive" : "Add";
                case BLEND_SUB:
                    return "Sub";
                default:
                    return $"Unknown ({GetBlendMode()})";
            }
        }
    }
}
