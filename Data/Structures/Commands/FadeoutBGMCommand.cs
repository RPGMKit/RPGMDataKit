using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class FadeoutBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.FadeoutBgm;
    public override string GetCommandName(StorageVersion version) => "Fadeout BGM";

    public int Seconds
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.Version == StorageVersion.MV ? $"{Seconds} second(s)" : $"{Seconds} sec.";
    }
}