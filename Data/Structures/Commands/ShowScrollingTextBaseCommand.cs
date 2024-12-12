using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowScrollingTextBaseCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowScrollingTextBase;
    public override string GetCommandName(StorageVersion version) => "Text(S)";

    public int Speed { get; set; }
    public bool NoFastForward { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Speed = parameterProvider.GetParameter<int>(0);
        NoFastForward = parameterProvider.GetParameter<bool>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Speed);
        parameterProvider.SetParameter(1, NoFastForward);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = $"Speed {Speed}";
        if (NoFastForward)
            str += ", No Fast Forward";

        return str;
    }
}