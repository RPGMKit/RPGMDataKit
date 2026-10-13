using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeParallaxCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeParallax;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Parallax";
        else
            return "Change Parallax Back";
    }
        
    public string Image
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public bool LoopHorizontally
    {
        get => Get<bool>(1);
        set => Set(1, value);
    }

    public bool LoopVertically
    {
        get => Get<bool>(2);
        set => Set(2, value);
    }

    public int HorizontalScroll
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public int VerticalScroll
    {
        get => Get<int>(4);
        set => Set(4, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.Version == StorageVersion.MV ? Image : $"'{Image}'";

        // only in MV down here
        List<string> p = new();
        if (LoopHorizontally)
            p.Add("Loop Horizontally");

        if (LoopVertically)
            p.Add("Loop Vertically");

        if (p.Count > 0)
            str += " (" + string.Join(", ", p) + ")";

        return str;
    }
}