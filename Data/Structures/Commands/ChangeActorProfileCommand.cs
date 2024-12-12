using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeActorProfileCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeActorProfile;

    public override string GetCommandName(StorageVersion version)
    {
        return "Change Profile";
    }

    public int ActorId { get; set; }
    public string Text { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ActorId = parameterProvider.GetParameter<int>(0);
        Text = parameterProvider.GetParameter<string>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ActorId);
        parameterProvider.SetParameter(1, Text);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{context.NameFormatter.FormatActor(ActorId)}, {Text}";
    }
}
