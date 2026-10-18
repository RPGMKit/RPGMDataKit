using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeBattleBackground : NamedCommand
{
    public override int Code => CommandCodes.ChangeBattleBackground;
    public override string GetCommandName(StorageVersion version) => "Change Battle Back";

    [EditorDefault("")]
    public string Image1
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    [EditorDefault("")]
    public string Image2
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return !string.IsNullOrEmpty(Image2) ? $"{Image1} & {Image2}" : Image1;
    }
}