using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;


public class ChangeWindowColorCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeWindowColor;
    public override string GetCommandName(StorageVersion version) => "Change Window Color";

    public ITone Tone
    {
        get => GetEntity<ITone>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"({Tone.Red},{Tone.Green},{Tone.Blue})";
    }
}