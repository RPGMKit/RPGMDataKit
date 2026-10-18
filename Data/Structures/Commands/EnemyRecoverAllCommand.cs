using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class EnemyRecoverAllCommand : NamedCommand
{
    public override int Code => CommandCodes.EnemyRecoverAll;
    public override string GetCommandName(StorageVersion version) => "Enemy Recover All";

    [EditorDefault(-1, StorageVersion.VXAce)]
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