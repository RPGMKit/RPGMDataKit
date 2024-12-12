using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeParallaxCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeParallaxBackground;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Parallax";
        else
            return "Change Parallax Back";
    }
        
    public string Image { get; set; }
    public bool LoopHorizontally { get; set; }
    public bool LoopVertically { get; set; }
    public int HorizontalScroll { get; set; }
    public int VerticalScroll { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Image = parameterProvider.GetParameter<string>(0);
        LoopHorizontally = parameterProvider.GetParameter<bool>(1);
        LoopVertically = parameterProvider.GetParameter<bool>(2);
        HorizontalScroll = parameterProvider.GetParameter<int>(3);
        VerticalScroll = parameterProvider.GetParameter<int>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Image);
        parameterProvider.SetParameter(1, LoopHorizontally);
        parameterProvider.SetParameter(2, LoopVertically);
        parameterProvider.SetParameter(3, HorizontalScroll);
        parameterProvider.SetParameter(4, VerticalScroll);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.Version == StorageVersion.MV ? Image : $"'{Image}'";

        // only in MV down here
        List<string> p = new();
        if (LoopHorizontally)
            p.Add("Loop Horizontally");

        if (LoopVertically)
            p.Add("Loop Vertically");

        if (p.Count > 0)
            str += " (" + string.Join(", ", p) + ")";

        return str;
    }
}