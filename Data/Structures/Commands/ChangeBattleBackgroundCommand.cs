using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeBattleBackground : NamedCommand
{
    public override int Code => CommandCodes.ChangeBattleBackground;
    public override string GetCommandName(StorageVersion version) => "Change Battle Back";

    public string Image1
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

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