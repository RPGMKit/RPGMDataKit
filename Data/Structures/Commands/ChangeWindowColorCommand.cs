using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;


public class ChangeWindowColorCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeWindowColor;
    public override string GetCommandName(StorageVersion version) => "Change Window Color";

    public ITone Tone
    {
        get => GetEntity<ITone>(0);
        set => Set(0, IsDetached ? value : Tones.For(value, Map));
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"({Tone.Red},{Tone.Green},{Tone.Blue})";
    }
}