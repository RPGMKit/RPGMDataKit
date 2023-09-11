using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveSpeedCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Speed;
        public override string Name => "Speed";

        public int GetSpeed()
        {
            return (int)GetParameter(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetSpeed().ToString();
        }
    }
}
