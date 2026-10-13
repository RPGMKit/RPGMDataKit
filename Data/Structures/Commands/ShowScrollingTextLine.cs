using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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