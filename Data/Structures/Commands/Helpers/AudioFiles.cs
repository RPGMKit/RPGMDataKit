using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Classes;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;

/// <summary>The sound object a command stores: <see cref="SoundData"/> in MV, an <see cref="AudioFile"/> of its kind in VXAce.</summary>
internal static class AudioFiles
{
    /// <summary>The sound as MV stores it; <paramref name="sound"/> itself when it already is.</summary>
    public static IAudioFile? ForMv(IAudioFile? sound)
    {
        if (sound == null)
            return null;

        return sound as SoundData ?? new SoundData { Name = sound.Name, Volume = sound.Volume, Pitch = sound.Pitch };
    }

    /// <summary>The sound as VXAce stores it; <paramref name="sound"/> itself when it already is a <typeparamref name="T"/>.</summary>
    public static IAudioFile ForVxAce<T>(IAudioFile sound) where T : AudioFile, new()
    {
        return sound as T ?? new T { Name = sound.Name, Volume = sound.Volume, Pitch = sound.Pitch };
    }
}
