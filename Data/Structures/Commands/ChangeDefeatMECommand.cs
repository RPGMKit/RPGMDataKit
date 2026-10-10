using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeDefeatME : NamedCommand
{
    public override int Code => CommandCodes.ChangeDefeatME;

    public override string GetCommandName(StorageVersion version) => "Change Defeat ME";
    
    public IAudioFile SoundData
    {
        get => GetEntity<IAudioFile, SoundData>(0);
        set => Set(0, AudioFiles.ForMv(value));
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        if (version != StorageVersion.MV)
            throw new Exception("Exists only in MV");
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}
