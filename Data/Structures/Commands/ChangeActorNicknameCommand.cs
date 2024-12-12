using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeActorNicknameCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeActorNickname;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Nickname";
        else
            return "Change Actor Nickname";
    }

    public int ActorId { get; set; }
    public string NickName { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ActorId = parameterProvider.GetParameter<int>(0);
        NickName = parameterProvider.GetParameter<string>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ActorId);
        parameterProvider.SetParameter(1, NickName);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (context.Version == StorageVersion.MV)
            return $"{context.NameFormatter.FormatActor(ActorId)}, {NickName}";
        else
            return $"{context.NameFormatter.FormatActor(ActorId)}, '{NickName}'";
    }
}
