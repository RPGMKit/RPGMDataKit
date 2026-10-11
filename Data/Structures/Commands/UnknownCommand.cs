using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

/// <summary>A command with a code this library does not know; its parameters are available as they are.</summary>
public class UnknownCommand : NamedCommand
{
    private readonly int _code;

    public UnknownCommand(int code)
    {
        _code = code;
    }

    public override int Code => _code;

    public override string GetCommandName(StorageVersion version) => $"Unknown ({Code})";

    public IParameterProvider Parameters => GetParameterProvider();

    // there are no properties to copy: the parameters are, as they are (JSON objects are copied)
    protected override NamedCommand CreateBlank() => new UnknownCommand(Code)
    {
        Raw = { Parameters = Raw.Parameters?.Select(p => p is JToken token ? token.DeepClone() : p).ToList() },
    };

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        throw new Exception("not implemented");
    }
}
