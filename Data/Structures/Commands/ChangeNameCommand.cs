using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeNameCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeName;
    public override string GetCommandName(StorageVersion version) => "Change Name";

    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public string Name
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatActor(ActorId);
        if (context.Version == StorageVersion.MV)
            str += $", {Name}";
        else
            str += $", '{Name}'";

        return str;
    }
}