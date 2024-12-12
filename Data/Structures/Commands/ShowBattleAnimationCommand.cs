using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowBattleAnimationCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowBattleAnimation;
    public override string GetCommandName(StorageVersion version) => "Show Battle Animation";
    
    public int TroopId { get; set; }
    public int AnimationId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        TroopId = parameterProvider.GetParameter<int>(0);
        AnimationId = parameterProvider.GetParameter<int>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, TroopId);
        parameterProvider.SetParameter(1, AnimationId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(TroopId);

        str += $", {context.NameFormatter.FormatAnimation(AnimationId)}";

        return str;
    }
}