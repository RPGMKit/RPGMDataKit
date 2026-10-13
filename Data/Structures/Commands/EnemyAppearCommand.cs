using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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