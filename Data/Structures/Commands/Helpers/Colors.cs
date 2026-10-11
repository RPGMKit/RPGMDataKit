using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;

internal static class Colors
{
    /// <summary>
    /// The color as the type map stores <see cref="IColor"/>: <paramref name="color"/> itself when it already is,
    /// else a new one with the same values (e.g. a color of the other maker version).
    /// </summary>
    public static IColor? For(IColor? color, TypeMap map)
    {
        if (color == null)
            return null;

        if (map.Resolve(typeof(IColor)).IsInstanceOfType(color))
            return color;

        var converted = map.Factory<IColor>()();
        converted.Red = color.Red;
        converted.Green = color.Green;
        converted.Blue = color.Blue;
        converted.Alpha = color.Alpha;

        return converted;
    }
}
