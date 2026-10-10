using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChoiceCaseCommand : NamedCommand
{
    public ChoiceCaseCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.ChoiceCase;
    public override string GetCommandName(StorageVersion version) => "When";

    public int ChoiceIndex
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public string ChoiceName
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.Version == StorageVersion.MV ? ChoiceName : $"[{ChoiceName}]";
    }
}