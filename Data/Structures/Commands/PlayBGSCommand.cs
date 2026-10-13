using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class PlayBGSCommand : NamedCommand
{
    public override int Code => CommandCodes.PlayBgs;
    public override string GetCommandName(StorageVersion version) => "Play BGS";
    
    public IBGS SoundData
    {
        get => GetEntity<IBGS>(0);
        set => Set(0, value);
    }

        
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}