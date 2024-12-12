using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class InputNumberCommand : NamedCommand
{
    public override int Code => CommandCodes.InputNumber;
    public override string GetCommandName(StorageVersion version) => "Input Number";

    public int VariableId { get; set; }
    public int DigitsCount { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        VariableId = parameterProvider.GetParameter<int>(0);
        DigitsCount = parameterProvider.GetParameter<int>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, VariableId);
        parameterProvider.SetParameter(1, DigitsCount);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var variableName = context.NameFormatter.FormatVariable(VariableId, new()
        {
            BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
        });

        var str = $"{variableName}, {DigitsCount} digits";

        return str;
    }
}