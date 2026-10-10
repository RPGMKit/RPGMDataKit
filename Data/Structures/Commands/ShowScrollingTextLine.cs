using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowScrollingTextLine : NamedCommand
{
    public ShowScrollingTextLine()
    {
        IsSecondary = true;
    }

    public override string GetCommandName(StorageVersion version) => "    ";
        
    public override int Code => CommandCodes.ShowScrollingTextLine;

    public string Text
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Text;
}