using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages;

namespace RPGMDataKit.Data.Structures.Commands.Helpers;

internal static class AudioFiles
{
    /// <summary>
    /// The sound as the type map stores <typeparamref name="T"/>: <paramref name="sound"/> itself when it already is,
    /// else a new one with the same name, volume and pitch (e.g. a sound of the other maker version).
    /// </summary>
    public static T? For<T>(T? sound, TypeMap map) where T : class, IAudioFile
    {
        if (sound == null)
            return null;

        if (map.Resolve(typeof(T)).IsInstanceOfType(sound))
            return sound;

        var converted = map.Factory<T>()();
        converted.Name = sound.Name;
        converted.Volume = sound.Volume;
        converted.Pitch = sound.Pitch;

        return converted;
    }
}
