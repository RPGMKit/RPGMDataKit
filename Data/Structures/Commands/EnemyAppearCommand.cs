using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class EnemyAppearCommand : NamedCommand
    {
        public int GetEnemy()
        {
            return GetParameter<int>(0);
        }

        public override int Code => CommandCodes.EnemyAppear;
        
        public override string GetCommandName(StorageVersion version) => "Enemy Appear";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return $"#{GetEnemy() + 1}";
        }
    }
}
