using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;

/// <summary>The tone object a command stores: an array object in MV, a tone object in VXAce.</summary>
internal static class Tones
{
    /// <summary>The tone as the given version stores it; <paramref name="tone"/> itself when it already is.</summary>
    public static ITone? For(ITone? tone, StorageVersion version)
    {
        if (tone == null)
            return null;

        if (version == StorageVersion.MV)
            return tone as MV.Classes.Tone ??
                   new MV.Classes.Tone { Red = tone.Red, Green = tone.Green, Blue = tone.Blue, Gray = tone.Gray };

        return tone as VXAce.Classes.Custom.Tone ??
               new VXAce.Classes.Custom.Tone { Red = tone.Red, Green = tone.Green, Blue = tone.Blue, Gray = tone.Gray };
    }
}
