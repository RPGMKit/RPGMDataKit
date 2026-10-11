using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeBattleBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeBattleBGM;
    public override string GetCommandName(StorageVersion version) => "Change Battle BGM";
    
    public IBGM SoundData
    {
        get => GetEntity<IBGM>(0);
        set => Set(0, IsDetached ? value : AudioFiles.For(value, Map));
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}
