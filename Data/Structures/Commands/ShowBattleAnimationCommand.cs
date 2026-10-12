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

    // exists only in MV 1.3.5 and later (a command of an older file has two parameters, and it reads as false); when it is set,
    // the enemy of the first parameter is ignored. A command made in code keeps it until it is put into a VXAce list
    public bool EntireTroop
    {
        get => Get<bool>(2);
        set
        {
            if (Version == StorageVersion.MV || IsDetached)
                Set(2, value);
        }
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        if (version != StorageVersion.MV)
            Raw.Parameters!.RemoveRange(2, Raw.Parameters.Count - 2);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(EntireTroop ? -1 : TroopId);

        str += $", {context.NameFormatter.FormatAnimation(AnimationId)}";

        return str;
    }
}