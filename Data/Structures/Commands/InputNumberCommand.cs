using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class InputNumberCommand : NamedCommand
{
    public override int Code => CommandCodes.InputNumber;
    public override string GetCommandName(StorageVersion version) => "Input Number";

    [EditorDefault(1, StorageVersion.VXAce)]
    public int VariableId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    [EditorDefault(1, StorageVersion.VXAce)]
    public int DigitsCount
    {
        get => Get<int>(1);
        set => Set(1, value);
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