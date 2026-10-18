using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ScrollMapCommand : NamedCommand
{
    public override int Code => CommandCodes.ScrollMap;
    public override string GetCommandName(StorageVersion version) => "Scroll Map";

    [EditorDefault(2, StorageVersion.VXAce)]
    public Direction Direction
    {
        get => Get<Direction>(0);
        set => Set(0, value);
    }

    [EditorDefault(1, StorageVersion.VXAce)]
    public int Distance
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    [EditorDefault(4, StorageVersion.VXAce)]
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