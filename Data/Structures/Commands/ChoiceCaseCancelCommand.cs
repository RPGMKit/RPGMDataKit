using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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

    // what the editors write: MV has 6 (the number of the Cancel choice) and null, VX Ace nothing
    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        if (Raw.Parameters is { Count: 2 } parameters && parameters[0] == null && parameters[1] == null)
        {
            parameters.Clear();
            if (version == StorageVersion.MV)
                parameters.AddRange([6, null]);
        }
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return "Cancel";
    }
}