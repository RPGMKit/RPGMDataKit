using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfEventCommand : IfCommand
{
    public int EventId { get; set; }
    public Direction Direction { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        EventId = parameterProvider.GetParameter<int>(1);
        Direction = parameterProvider.GetParameter<Direction>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, EventId);
        parameterProvider.SetParameter(2, Direction);
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