using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class EnemyRecoverAllCommand : NamedCommand
{
    public override int Code => CommandCodes.EnemyRecoverAll;
    public override string GetCommandName(StorageVersion version) => "Enemy Recover All";

    public int GetMember() => GetParameter<int>(0);

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatTroopMember(GetMember());
    }
}