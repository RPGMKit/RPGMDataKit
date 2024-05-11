using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ScrollMapCommand : NamedCommand
    {
        public override int Code => CommandCodes.ScrollMap;
        public override string GetCommandName(StorageVersion version) => "Scroll Map";

        public Direction GetDirection()
        {
            return (Direction)GetParameter<int>(0);
        }

        public int GetDistance()
        {
            return GetParameter<int>(1);
        }

        public int GetSpeed()
        {
            return GetParameter<int>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = "";
            switch (GetDirection())
            {
                case Direction.Down:
                    str = "Down";
                    break;
                case Direction.Left:
                    str = "Left";
                    break;
                case Direction.Right:
                    str = "Right";
                    break;
                case Direction.Up:
                    str = "Up";
                    break;
                default:
                    throw new Exception($"Unknown direction '{GetDirection()}'");
            }

            str += $", {GetDistance()}, {GetSpeed()}";

            return str;
        }
    }
}
