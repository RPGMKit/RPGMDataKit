using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;

internal static class Tones
{
    /// <summary>
    /// The tone as the type map stores <see cref="ITone"/>: <paramref name="tone"/> itself when it already is,
    /// else a new one with the same values (e.g. a tone of the other maker version).
    /// </summary>
    public static ITone? For(ITone? tone, TypeMap map)
    {
        if (tone == null)
            return null;

        if (map.Resolve(typeof(ITone)).IsInstanceOfType(tone))
            return tone;

        var converted = map.Factory<ITone>()();
        converted.Red = tone.Red;
        converted.Green = tone.Green;
        converted.Blue = tone.Blue;
        converted.Gray = tone.Gray;

        return converted;
    }
}
