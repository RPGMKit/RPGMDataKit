using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChoiceCaseCancelCommand : NamedCommand
{
    public ChoiceCaseCancelCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.ChoiceCaseCancel;
    public override string GetCommandName(StorageVersion version) => "When";

    public object Unk0
    {
        get => Get<object>(0);
        set => Set(0, value);
    }

    public object Unk1
    {
        get => Get<object>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return "Cancel";
    }
}