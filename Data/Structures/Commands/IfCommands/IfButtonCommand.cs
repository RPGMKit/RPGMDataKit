using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfButtonCommand : IfCommand
{
    public enum Buttons
    {
        Down = 2,
        Left = 4,
        Right = 6,
        Up = 8,
        A = 11,
        B = 12,
        C = 13,
        X = 14,
        Y = 15,
        Z = 16,
        L = 17,
        R = 18,
    }

    private static readonly Dictionary<Buttons, string> ButtonIdToName = new()
    {
        [Buttons.Down] = "Down",
        [Buttons.Left] = "Left",
        [Buttons.Right] = "Right",
        [Buttons.Up] = "Up",
        [Buttons.A] = "A",
        [Buttons.B] = "B",
        [Buttons.C] = "C",
        [Buttons.X] = "X",
        [Buttons.Y] = "Y",
        [Buttons.Z] = "Z",
        [Buttons.L] = "L",
        [Buttons.R] = "R",
    };

    // the names of the buttons in MV (Input.isPressed): the buttons of VXAce that have one
    private static readonly Dictionary<Buttons, string> ButtonIdToMvName = new()
    {
        [Buttons.Down] = "down",
        [Buttons.Left] = "left",
        [Buttons.Right] = "right",
        [Buttons.Up] = "up",
        [Buttons.A] = "shift",
        [Buttons.B] = "cancel",
        [Buttons.C] = "ok",
        [Buttons.L] = "pageup",
        [Buttons.R] = "pagedown",
    };

    // a name in MV, a number in VXAce
    public Buttons Button
    {
        get => Version == StorageVersion.MV && Raw.Parameters?.Get(1) is string name
            ? FromMvName(name)
            : Get<Buttons>(1);
        set => Set(1, Version == StorageVersion.MV
            ? ButtonIdToMvName.GetValueOrDefault(value) ?? ButtonIdToName[value]
            : (object)value);
    }

    private static Buttons FromMvName(string name)
    {
        foreach (var (button, mvName) in ButtonIdToMvName)
        {
            if (string.Equals(mvName, name, StringComparison.OrdinalIgnoreCase))
                return button;
        }

        // names of the buttons of VXAce (A, B, C...), as older data may have them
        foreach (var (button, vxName) in ButtonIdToName)
        {
            if (vxName == name.UcFirst())
                return button;
        }

        throw new InvalidOperationException($"There is no button '{name}' that has a number in VXAce");
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        // a name of MV is shown as it is, so a button that VXAce has no number for (tab, control...) can be shown too
        var buttonName = Version == StorageVersion.MV && Raw.Parameters?.Get(1) is string mvName
            ? mvName.UcFirst()
            : ButtonIdToName.FirstOrDefault(_ => _.Key == Button).Value ?? $"#{Button}";

        return context.Version == StorageVersion.MV
            ? $"Button [{buttonName}] is pressed down"
            : $"The {buttonName} Button is Being Pressed";
    }
}