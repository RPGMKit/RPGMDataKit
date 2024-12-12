using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
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

    public Buttons Button { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        if (version == StorageVersion.MV)
            Button = ButtonIdToName.First(_ => _.Value == parameterProvider.GetParameter<string>(1).UcFirst()).Key;
        else 
            Button = (Buttons)parameterProvider.GetParameter<int>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(1, ButtonIdToName[Button]);
        else
            parameterProvider.SetParameter(1, (int)Button);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var buttonName = ButtonIdToName.FirstOrDefault(_ => _.Key == Button).Value ?? $"#{Button}";

        return context.Version == StorageVersion.MV
            ? $"Button [{buttonName}] is pressed down"
            : $"The {buttonName} Button is Being Pressed";
    }
}