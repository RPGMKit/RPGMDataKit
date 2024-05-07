using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveJumpCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Jump;
        public override string GetCommandName(StorageVersion version) => "Jump";
        public int GetXDelta()
        {
            return GetParameter<int>(0);
        }
        public int GetYDelta()
        {
            return GetParameter<int>(1);
        }
        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var x = GetXDelta() >= 0 ? $"+{GetXDelta()}" : GetXDelta().ToString();
            var y = GetYDelta() >= 0 ? $"+{GetYDelta()}" : GetYDelta().ToString();
    
            return $"{x}, {y}";
        }
    }
}