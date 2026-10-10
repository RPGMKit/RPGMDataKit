using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ScriptLineCommand : NamedCommand
{
    public ScriptLineCommand()
    {
        IsSecondary = true;
    }

    public override string GetCommandName(StorageVersion version) => "      ";
        
    public override int Code => CommandCodes.ScriptLine;

    public string Text
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Text;
}