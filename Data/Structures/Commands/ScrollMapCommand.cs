using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ScrollMapCommand : NamedCommand
{
    public override int Code => CommandCodes.ScrollMap;
    public override string GetCommandName(StorageVersion version) => "Scroll Map";

    public Direction Direction
    {
        get => Get<Direction>(0);
        set => Set(0, value);
    }

    public int Distance
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public int Speed
    {
        get => Get<int>(2);
        set => Set(2, value);
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