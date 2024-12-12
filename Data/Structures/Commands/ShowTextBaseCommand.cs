using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowTextBaseCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowTextBase;
    public override string GetCommandName(StorageVersion version) => "Text";
    
    public enum BackgroundType
    {
        Window = 0,
        Dim = 1,
        Transparent = 2,
    }

    public enum PositionType
    {
        Top = 0,
        Middle = 1,
        Bottom = 2,
    }
        
    public string Face { get; set; }
    public int Offset { get; set; }
    public BackgroundType Background { get; set; }
    public PositionType Position { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Face = parameterProvider.GetParameter<string>(0);
        Offset = parameterProvider.GetParameter<int>(1);
        Background = (BackgroundType)parameterProvider.GetParameter<int>(2);
        Position = (PositionType)parameterProvider.GetParameter<int>(3);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Face);
        parameterProvider.SetParameter(1, Offset);
        parameterProvider.SetParameter(2, (int)Background);
        parameterProvider.SetParameter(3, (int)Position);
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