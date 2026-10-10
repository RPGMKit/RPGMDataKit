using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

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