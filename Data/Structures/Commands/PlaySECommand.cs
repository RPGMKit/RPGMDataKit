using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PlaySECommand : NamedCommand
{
    public override int Code => CommandCodes.PlaySe;
    public override string GetCommandName(StorageVersion version) => "Play SE";
        
    public ISE SoundData
    {
        get => GetEntity<ISE>(0);
        set => Set(0, value);
    }

        
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}