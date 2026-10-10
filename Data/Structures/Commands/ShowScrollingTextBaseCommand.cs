using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowScrollingTextBaseCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowScrollingTextBase;
    public override string GetCommandName(StorageVersion version) => "Text(S)";

    public int Speed
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public bool NoFastForward
    {
        get => Get<bool>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = $"Speed {Speed}";
        if (NoFastForward)
            str += ", No Fast Forward";

        return str;
    }
}