using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class UnknownCommand : NamedCommand
{
    private readonly int _code;

    public UnknownCommand(int code, IParameterProvider parameters)
    {
        _code = code;
        Parameters = parameters;
    }

    public override int Code => _code;

    public override string GetCommandName(StorageVersion version) => $"Unknown ({Code})";
    
    public IParameterProvider Parameters { get; }
    
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        throw new Exception("not implemented");
        // return JsonConvert.SerializeObject(Parameters);
    }
}