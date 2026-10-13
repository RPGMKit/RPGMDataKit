using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class JumpToLabelCommand : NamedCommand
{
    public override int Code => CommandCodes.JumpToLabel;
    public override string GetCommandName(StorageVersion version) => "Jump to Label";

    public string Label
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Label;
}