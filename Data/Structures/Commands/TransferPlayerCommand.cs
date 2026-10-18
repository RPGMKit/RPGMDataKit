using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class TransferPlayerCommand : NamedCommand
{
    public override int Code => CommandCodes.TransferPlayer;
    public override string GetCommandName(StorageVersion version) => "Transfer Player";
    
    public enum DesignationType
    {
        Direct = 0,
        ByVariable = 1,
    }

    public enum FadeType
    {
        Black = 0,
        Normal = 0,
        White = 1,
        None = 2,
    }

    public DesignationType Designation
    {
        get => Get<DesignationType>(0);
        set => Set(0, value);
    }

    [EditorDefault(1, StorageVersion.VXAce)]
    public int Param1
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public int Param2
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public int Param3
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public Direction Direction
    {
        get => Get<Direction>(4);
        set => Set(4, value);
    }

    public FadeType Fade
    {
        get => Get<FadeType>(5);
        set => Set(5, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = "";
        switch (Designation)
        {
            case DesignationType.Direct:
                str = context.NameFormatter.FormatMap(Param1);

                if (context.Version == StorageVersion.MV)
                    str += $" ({Param2},{Param3})";
                else
                    str += $" ({Param2:D3},{Param3:D3})";
                break;
            case DesignationType.ByVariable:
                string var1, var2, var3;
                if (context.Version == StorageVersion.MV)
                {
                    var1 = context.NameFormatter.FormatVariable(Param1);
                    var2 = context.NameFormatter.FormatVariable(Param2);
                    var3 = context.NameFormatter.FormatVariable(Param3);

                    str = $"{var1} ({var2},{var3})";
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
                    var3 = context.NameFormatter.FormatVariable(Param3, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });

                    str = $"Variable {var1}{var2}{var3}";
                }

                break;
            default:
                throw new Exception($"Unknown transfer player transfer type '{Designation}'");
        }

        List<string> subPart = new();
        if (Direction != Direction.Retain)
        {
            var directionFmt = context.Version == StorageVersion.MV ? "Direction: {0}" : "{0}";
            subPart.Add(Direction switch
            {
                Direction.Down => string.Format(directionFmt, "Down"),
                Direction.Left => string.Format(directionFmt, "Left"),
                Direction.Right => string.Format(directionFmt, "Right"),
                Direction.Up => string.Format(directionFmt, "Up"),
                _ => throw new Exception($"Unknown direction {Direction}")
            });
        }

        if (Fade != FadeType.Black)
        {
            var fadeFmt = context.Version == StorageVersion.MV ? "Fade: {0}" : "{0}";
            subPart.Add(Fade switch
            {
                FadeType.White => string.Format(fadeFmt, "White"),
                FadeType.None => string.Format(fadeFmt, "None"),
                _ => throw new Exception($"Unknown fade '{Fade}'")
            });
        }

        if (subPart.Count > 0)
        {
            var joined = string.Join(", ", subPart);
            if (context.Version == StorageVersion.MV)
                str += $" ({joined})";
            else
                str += $", {joined}";
        }

        return str;
    }
}