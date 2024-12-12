using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeNameCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeName;
    public override string GetCommandName(StorageVersion version) => "Change Name";

    public int ActorId { get; set; }
    public string Name { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ActorId = parameterProvider.GetParameter<int>(0);
        Name = parameterProvider.GetParameter<string>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ActorId);
        parameterProvider.SetParameter(1, Name);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatActor(ActorId);
        if (context.Version == StorageVersion.MV)
            str += $", {Name}";
        else
            str += $", '{Name}'";

        return str;
    }
}