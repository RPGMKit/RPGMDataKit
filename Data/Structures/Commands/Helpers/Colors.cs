using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;

/// <summary>The color object a command stores: an array object in MV, a color object in VXAce.</summary>
internal static class Colors
{
    /// <summary>The color as the given version stores it; <paramref name="color"/> itself when it already is.</summary>
    public static IColor? For(IColor? color, StorageVersion version)
    {
        if (color == null)
            return null;

        if (version == StorageVersion.MV)
            return color as MV.Classes.Color ??
                   new MV.Classes.Color { Red = color.Red, Green = color.Green, Blue = color.Blue, Alpha = color.Alpha };

        return color as VXAce.Classes.Custom.Color ??
               new VXAce.Classes.Custom.Color { Red = color.Red, Green = color.Green, Blue = color.Blue, Alpha = color.Alpha };
    }
}
