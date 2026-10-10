using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowTextBaseCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowTextBase;
    public override string GetCommandName(StorageVersion version) => "Text";
    
    public enum PositionType
    {
        Top = 0,
        Middle = 1,
        Bottom = 2,
    }
        
    public string Face
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public int Offset
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public BackgroundType Background
    {
        get => Get<BackgroundType>(2);
        set => Set(2, value);
    }

    public PositionType Position
    {
        get => Get<PositionType>(3);
        set => Set(3, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var face = Face;
        if (string.IsNullOrWhiteSpace(face))
            face = context.Version == StorageVersion.MV ? "None" : "-, -";
        else
            face = context.Version == StorageVersion.MV ? $"{face}({Offset})" : $"'{face}', {Offset}";

        var background = Background switch
        {
            BackgroundType.Window => context.Version == StorageVersion.MV ? "Window" : "Normal",
            BackgroundType.Dim => context.Version == StorageVersion.MV ? "Dim" : "Dark",
            BackgroundType.Transparent => "Transparent",
            _ => throw new Exception($"Unknown background '{Background}'")
        };

        var position = Position switch
        {
            PositionType.Top => "Top",
            PositionType.Middle => "Middle",
            PositionType.Bottom => "Bottom",
            _ => throw new Exception($"Unknown position '{Position}'")
        };

        return $"{face}, {background}, {position}";
    }
}