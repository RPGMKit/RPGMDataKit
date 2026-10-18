using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class EnemyTransformCommand : NamedCommand
{
    public override int Code => CommandCodes.EnemyTransform;
    public override string GetCommandName(StorageVersion version) => "Enemy Transform";

    public int TroopId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    [EditorDefault(1)]
    public int EnemyId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }
    
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return
            $"{context.NameFormatter.FormatTroopMember(TroopId)}, {context.NameFormatter.FormatEnemy(EnemyId)}";
    }
}