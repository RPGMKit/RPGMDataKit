using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfEventCommand : IfCommand
    {
        public int GetEventId()
        {
            return GetParameter<int>(1);
        }

        public Direction GetDirection()
        {
            return (Direction)GetParameter<int>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatEvent(GetEventId());

            if (context.Version == StorageVersion.MV)
                str += " is facing ";
            else
                str += " is Facing ";
            switch (GetDirection())
            {
                case Direction.Down:
                    str += "Down";
                    break;
                case Direction.Left:
                    str += "Left";
                    break;
                case Direction.Right:
                    str += "Right";
                    break;
                case Direction.Up:
                    str += "Up";
                    break;
                default:
                    throw new Exception(
                        $"Unknown orientation '{GetDirection()}' for '{GetCommandName(context.Version)}' orientation command");
            }

            return str;
        }
    }
}
