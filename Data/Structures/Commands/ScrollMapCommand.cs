using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ScrollMapCommand : NamedCommand
{
    public override int Code => CommandCodes.ScrollMap;
    public override string GetCommandName(StorageVersion version) => "Scroll Map";

    public Direction Direction { get; set; }
    public int Distance { get; set; }
    public int Speed { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Direction = parameterProvider.GetParameter<Direction>(0);
        Distance = parameterProvider.GetParameter<int>(1);
        Speed = parameterProvider.GetParameter<int>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Direction);
        parameterProvider.SetParameter(1, Distance);
        parameterProvider.SetParameter(2, Speed);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Direction switch
        {
            Direction.Down => "Down",
            Direction.Left => "Left",
            Direction.Right => "Right",
            Direction.Up => "Up",
            _ => throw new Exception($"Unknown direction '{Direction}'")
        };

        str += $", {Distance}, {Speed}";

        return str;
    }
}