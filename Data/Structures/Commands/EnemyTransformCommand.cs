using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class EnemyTransformCommand : NamedCommand
{
    public override int Code => CommandCodes.EnemyTransform;
    public override string GetCommandName(StorageVersion version) => "Enemy Transform";

    public int GetMember() => GetParameter<int>(0);

    public int GetEnemy() => GetParameter<int>(1);

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return
            $"{context.NameFormatter.FormatTroopMember(GetMember())}, {context.NameFormatter.FormatEnemy(GetEnemy())}";
    }
}