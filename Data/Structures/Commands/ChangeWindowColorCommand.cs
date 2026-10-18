using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;


public class ChangeWindowColorCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeWindowColor;
    public override string GetCommandName(StorageVersion version) => "Change Window Color";

    // VX Ace: the editor opens with the window color of a new project; MV: the editor was not looked at, the tone is made as a new one
    [EditorDefaultTone(-34, 0, 68, 0, StorageVersion.VXAce)]
    [EditorDefaultTone(0, 0, 0, 0)]
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