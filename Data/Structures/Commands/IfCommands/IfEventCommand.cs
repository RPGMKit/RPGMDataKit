using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfEventCommand : IfCommand
{
    public int EventId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public Direction Direction
    {
        get => Get<Direction>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatEvent(EventId);

        if (context.Version == StorageVersion.MV)
            str += " is facing ";
        else
            str += " is Facing ";

        str += Direction switch
        {
            Direction.Down => "Down",
            Direction.Left => "Left",
            Direction.Right => "Right",
            Direction.Up => "Up",
            _ => throw new Exception(
                $"Unknown orientation '{Direction}' for '{GetCommandName(context.Version)}' orientation command")
        };

        return str;
    }
}