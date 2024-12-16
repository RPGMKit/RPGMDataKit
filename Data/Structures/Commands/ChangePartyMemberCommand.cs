using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangePartyMemberCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangePartyMember;
    public override string GetCommandName(StorageVersion version) => "Change Party Member";
        
    public enum ChangeType
    {
        Add = 0,
        Remove = 1,
    }

    public int ActorId { get; set; }
    public ChangeType Type { get; set; }
    public bool DoInitialize { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ActorId = parameterProvider.GetParameter<int>(0);
        Type = parameterProvider.GetParameter<ChangeType>(1);
        DoInitialize = version == StorageVersion.MV
            ? parameterProvider.GetParameter<bool>(2)
            : parameterProvider.GetParameter<int>(2) > 0;
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ActorId);
        parameterProvider.SetParameter(1, Type);
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(2, DoInitialize);
        else
            parameterProvider.SetParameter(2, DoInitialize ? 1 : 0);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Type switch
        {
            ChangeType.Add => "Add",
            ChangeType.Remove => "Remove",
            _ => throw new Exception($"Unknown add/remove state for party member")
        };

        str += context.NameFormatter.FormatActor(ActorId);
        if (DoInitialize)
            str += context.Version == StorageVersion.MV ? " (Initialize)" : ", Initialize";

        return str;
    }
}