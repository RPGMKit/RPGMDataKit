using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeBattleBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeBattleBGM;
    public override string GetCommandName(StorageVersion version) => "Change Battle BGM";
    
    public IBGM SoundData
    {
        get => GetEntity<IBGM>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}
