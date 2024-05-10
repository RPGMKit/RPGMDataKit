using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfEventCommand : IfCommand
    {
        protected const int ORIENTATION_DOWN = 2;
        protected const int ORIENTATION_LEFT = 4;
        protected const int ORIENTATION_RIGHT = 6;
        protected const int ORIENTATION_UP = 8;

        public int GetEventId()
        {
            return GetParameter<int>(1);
        }

        public int GetOrientation()
        {
            return GetParameter<int>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatEvent(GetEventId());

            if (context.Version == StorageVersion.MV)
                str += " is facing ";
            else
                str += " is Facing ";
            switch (GetOrientation())
            {
                case ORIENTATION_DOWN:
                    str += "Down";
                    break;
                case ORIENTATION_LEFT:
                    str += "Left";
                    break;
                case ORIENTATION_RIGHT:
                    str += "Right";
                    break;
                case ORIENTATION_UP:
                    str += "Up";
                    break;
                default:
                    throw new Exception(
                        $"Unknown orientation '{GetOrientation()}' for '{GetCommandName(context.Version)}' orientation command");
            }

            return str;
        }
    }
}
