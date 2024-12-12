using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeClassCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeClass;
    public override string GetCommandName(StorageVersion version) => "Change Class";

    public int ActorId { get; set; }
    public int ClassId { get; set; }
    public int Unk { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ActorId = parameterProvider.GetParameter<int>(0);
        ClassId = parameterProvider.GetParameter<int>(1);
        Unk = parameterProvider.GetParameter<int>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ActorId);
        parameterProvider.SetParameter(1, ClassId);
        parameterProvider.SetParameter(2, Unk);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{context.NameFormatter.FormatActor(ActorId)}, {context.NameFormatter.FormatClass(ClassId)}";
    }
}