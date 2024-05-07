using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveBlendModeCommand : AbstractMoveCommand
    {
        protected const int BLEND_NORMAL = 0;
        protected const int BLEND_ADDITIVE = 1;

        public override int Code => MovementRouteCommandCodes.BlendMode;
        public override string GetCommandName(StorageVersion version) => "Blend Mode";

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
                    return "Additive";
                default:
                    return $"Unknown ({GetBlendMode()})";
            }
        }
    }
}
