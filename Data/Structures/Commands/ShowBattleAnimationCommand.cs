using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowBattleAnimationCommand : NamedCommand
    {
        public int GetEnemy()
        {
            return GetParameter<int>(0);
        }

        public int GetAnimationId()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.ShowBattleAnimation;
        public override string GetCommandName(StorageVersion version) => "Show Battle Animation";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var source = GetEnemy();
            var str = "";
            if (source == -1)
                str = "Entire Troop";
            else
                str = $"#{source + 1}";

            str += $", {storage.NameFormatter.FormatAnimation(GetAnimationId())}";

            return str;
        }
    }
}
