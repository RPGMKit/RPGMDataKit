using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowBattleAnimationCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowBattleAnimation;
    public override string GetCommandName(StorageVersion version) => "Show Battle Animation";
    
    public int TroopId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public int AnimationId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(TroopId);

        str += $", {context.NameFormatter.FormatAnimation(AnimationId)}";

        return str;
    }
}