using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowChoicesCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowChoices;
    public override string GetCommandName(StorageVersion version) => "Show Choices";
    
    public enum BackgroundType
    {
        Window = 0,
        Dim = 1,
        Transparent = 2,
    }

    public enum PositionType
    {
        Left = 0,
        Middle = 1,
        Right = 2,
    }

    protected const int DefaultChoiceNone = -1;
    protected const int CancelChoiceBranch = -2;
    protected const int CancelChoiceDisallow = -1;

    public List<string> Choices { get; set; }
    public int CancelChoiceIndex { get; set; }
    public int DefaultChoiceIndex { get; set; }
    public PositionType Position { get; set; }
    public BackgroundType Background { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Choices = parameterProvider.GetParameter<List<string>>(0);;
        CancelChoiceIndex = parameterProvider.GetParameter<int>(1);
        DefaultChoiceIndex = parameterProvider.GetParameter<int>(2);
        Position = (PositionType)parameterProvider.GetParameter<int>(3);
        Background = (BackgroundType)parameterProvider.GetParameter<int>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Choices);
        parameterProvider.SetParameter(1, CancelChoiceIndex);
        parameterProvider.SetParameter(2, DefaultChoiceIndex);
        parameterProvider.SetParameter(3, (int)Position);
        parameterProvider.SetParameter(4, (int)Background);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var background = Background switch
        {
            BackgroundType.Window => "Window",
            BackgroundType.Dim => "Dim",
            BackgroundType.Transparent => "Transparent",
            _ => throw new Exception($"Unknown background '{Background}'")
        };

        var position = Position switch
        {
            PositionType.Right => "Right",
            PositionType.Middle => "Middle",
            PositionType.Left => "Left",
            _ => throw new Exception($"Unknown position '{Position}'")
        };

        var defaultChoice = DefaultChoiceIndex switch
        {
            DefaultChoiceNone => "-",
            _ => "#" + (DefaultChoiceIndex + 1)
        };

        var cancelChoice = CancelChoiceIndex switch
        {
            CancelChoiceBranch or CancelChoiceDisallow => "-",
            _ => "#" + (CancelChoiceIndex + 1)
        };

        return string.Join(", ", Choices) + $", ({background}, {position}, {defaultChoice}, {cancelChoice})";
    }
}