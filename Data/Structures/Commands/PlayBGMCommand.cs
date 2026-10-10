using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PlayBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.PlayBgm;
    public override string GetCommandName(StorageVersion version) => "Play BGM";


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
                Set(0, AudioFiles.ForVxAce<BGM>(value));
        }
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }

}