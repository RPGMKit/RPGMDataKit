using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatTroopMember(GetEnemy());

            str += $", {context.NameFormatter.FormatAnimation(GetAnimationId())}";

            return str;
        }
    }
}
