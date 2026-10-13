using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class SetEventLocationCommand : NamedCommand
{
    public override int Code => CommandCodes.SetEventLocation;
    public override string GetCommandName(StorageVersion version) => "Set Event Location";
    
    public enum DesignationType
    {
        Direct = 0,
        ByVariable = 1,
        Exchange = 2,
    }
    
    public int EventId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public DesignationType Designation
    {
        get => Get<DesignationType>(1);
        set => Set(1, value);
    }

    public int Param1
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public int Param2
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public Direction Direction
    {
        get => Get<Direction>(4);
        set => Set(4, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatEvent(EventId);

        switch (Designation)
        {
            case DesignationType.Direct:
                if (context.Version == StorageVersion.MV)
                    str += $", ({Param1},{Param2}";
                else
                    str += $", ({Param1:D3},{Param2:D3}";
                break;
            case DesignationType.ByVariable:
                string var1, var2;
                if (context.Version == StorageVersion.MV)
                {
                    var1 = context.NameFormatter.FormatVariable(Param1);
                    var2 = context.NameFormatter.FormatVariable(Param2);

                    str += $", ({var1},{var2})";
                }
                else
                {
                    var1 = context.NameFormatter.FormatVariable(Param1, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });
                    var2 = context.NameFormatter.FormatVariable(Param2, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });

                    str += $", Variable {var1}{var2}";
                }
                break;
            case DesignationType.Exchange:
                if (context.Version == StorageVersion.MV)
                    str += $", Exchange with {context.NameFormatter.FormatEvent(Param1)}";
                else
                    str += $", Switch with {context.NameFormatter.FormatEvent(Param1)}";
                break;
            default:
                throw new Exception($"Unknown set event location transfer type '{Designation}'");
        }

        var directionFmt = context.Version == StorageVersion.MV ? " (Direction: {0})" : ", {0}";

        str += Direction switch
        {
            Direction.Retain => "",
            Direction.Down => string.Format(directionFmt, "Down"),
            Direction.Left => string.Format(directionFmt, "Left"),
            Direction.Right => string.Format(directionFmt, "Right"),
            Direction.Up => string.Format(directionFmt, "Up"),
            _ => throw new Exception($"Unknown direction {Direction}")
        };

        return str;
    }
}