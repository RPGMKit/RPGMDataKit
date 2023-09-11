using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveOpacityCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Opacity;
        public override string Name => "Opacity";

        public int GetOpacity()
        {
            return (int)GetParameter(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetOpacity().ToString();
        }
    }
}
