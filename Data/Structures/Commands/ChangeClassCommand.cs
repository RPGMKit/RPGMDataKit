using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeClassCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeClass;
    public override string GetCommandName(StorageVersion version) => "Change Class";

    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public int ClassId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public int Unk
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{context.NameFormatter.FormatActor(ActorId)}, {context.NameFormatter.FormatClass(ClassId)}";
    }
}