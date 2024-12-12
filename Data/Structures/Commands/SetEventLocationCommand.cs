using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

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
    
    public int EventId { get; set; }
    public DesignationType Designation { get; set; }
    public int Param1 { get; set; }
    public int Param2 { get; set; }
    public Direction Direction { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        EventId = parameterProvider.GetParameter<int>(0);
        Designation = (DesignationType)parameterProvider.GetParameter<int>(1);
        Param1 = parameterProvider.GetParameter<int>(2);
        Param2 = parameterProvider.GetParameter<int>(3);
        Direction = (Direction)parameterProvider.GetParameter<int>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, EventId);
        parameterProvider.SetParameter(1, (int)Designation);
        parameterProvider.SetParameter(2, Param1);
        parameterProvider.SetParameter(3, Param2);
        parameterProvider.SetParameter(4, (int)Direction);
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