using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

public abstract class AbstractAudioCommand : NamedCommand
{
    public IAudioFile GetAudioData(int index, StorageVersion version)
    {
        return version == StorageVersion.MV ? GetParameter<SoundData>(index) : GetParameter<IAudioFile>(index);
    }

    protected string FormatSoundData(int index, EntityContext context, DumpOptions options)
    {
        var audioData = GetAudioData(index, context.Version);

        if (context.Version == StorageVersion.MV)
        {
            if (string.IsNullOrWhiteSpace(audioData.Name))
                return "None";

            return $"{audioData.Name} ({audioData.Volume}, {audioData.Pitch}, {audioData.Pan})";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(audioData.Name))
                return "(None)";

            return $"'{audioData.Name}', {audioData.Volume}, {audioData.Pitch}";
        } 
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return FormatSoundData(0, context, options);
    }
}