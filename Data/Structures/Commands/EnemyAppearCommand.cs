using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class EnemyAppearCommand : NamedCommand
{
    public override int Code => CommandCodes.EnemyAppear;
        
    public override string GetCommandName(StorageVersion version) => "Enemy Appear";

    public int TroopId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatTroopMember(TroopId);
    }
}