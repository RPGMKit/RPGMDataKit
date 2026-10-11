using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowChoicesCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowChoices;
    public override string GetCommandName(StorageVersion version) => "Show Choices";
    
    public enum PositionType
    {
        Left = 0,
        Middle = 1,
        Right = 2,
    }

    protected const int DefaultChoiceNone = -1;
    protected const int CancelChoiceBranch = -2;
    protected const int CancelChoiceDisallow = -1;

    // the list stored in the raw command, so changing it changes the command
    public List<string> Choices
    {
        get => GetEntity<List<string>>(0);
        set => Set(0, value);
    }

    public int CancelChoiceIndex
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public int DefaultChoiceIndex
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public PositionType Position
    {
        get => Get<PositionType>(3);
        set => Set(3, value);
    }

    public BackgroundType Background
    {
        get => Get<BackgroundType>(4);
        set => Set(4, value);
    }

    public bool IsCancelChoiceBranch => CancelChoiceIndex == CancelChoiceBranch;

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