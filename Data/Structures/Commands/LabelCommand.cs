using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class LabelCommand : NamedCommand
{
    public override int Code => CommandCodes.Label;
    public override string GetCommandName(StorageVersion version) => "Label";

    public string Label
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Label;
}