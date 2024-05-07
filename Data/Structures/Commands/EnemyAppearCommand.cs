using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

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

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"#{GetEnemy() + 1}";
        }
    }
}
