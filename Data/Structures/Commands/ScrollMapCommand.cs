using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ScrollMapCommand : NamedCommand
    {
        protected const int DIRECTION_DOWN = 2;
        protected const int DIRECTION_LEFT = 4;
        protected const int DIRECTION_RIGHT = 6;
        protected const int DIRECTION_UP = 8;

        public override int Code => CommandCodes.ScrollMap;
        public override string GetCommandName(StorageVersion version) => "Scroll Map";

        public int GetDirection()
        {
            return GetParameter<int>(0);
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
                case DIRECTION_DOWN:
                    str = "Down";
                    break;
                case DIRECTION_LEFT:
                    str = "Left";
                    break;
                case DIRECTION_RIGHT:
                    str = "Right";
                    break;
                case DIRECTION_UP:
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
