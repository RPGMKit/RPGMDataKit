using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowAnimationCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowAnimation;
    public override string GetCommandName(StorageVersion version) => "Show Animation";

    public int EventId { get; set; }
    public int AnimationId { get; set; }
    public bool WaitForCompletion { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        EventId = parameterProvider.GetParameter<int>(0);
        AnimationId = parameterProvider.GetParameter<int>(1);
        WaitForCompletion = parameterProvider.GetParameter<bool>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, EventId);
        parameterProvider.SetParameter(1, AnimationId);
        parameterProvider.SetParameter(2, WaitForCompletion);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatEvent(EventId);

        str += ", " + context.NameFormatter.FormatAnimation(AnimationId);

        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}