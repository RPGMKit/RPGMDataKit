using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowTextLineCommand : NamedCommand
{
    public ShowTextLineCommand()
    {
        IsSecondary = true;
    }

    public override string GetCommandName(StorageVersion version) => "    ";
        
    public override int Code => CommandCodes.ShowTextLine;

    public string Text
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Text;
}