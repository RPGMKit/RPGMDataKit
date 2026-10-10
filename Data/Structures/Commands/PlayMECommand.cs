using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PlayMECommand : NamedCommand
{
    public override int Code => CommandCodes.PlayMe;
    public override string GetCommandName(StorageVersion version) => "Play ME";
    
    public IAudioFile SoundData
    {
        get => GetEntity<IAudioFile, SoundData>(0);
        set
        {
            if (IsDetached || value == null)
                Set(0, value);
            else if (Version == StorageVersion.MV)
                Set(0, AudioFiles.ForMv(value));
            else
                Set(0, AudioFiles.ForVxAce<ME>(value));
        }
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}