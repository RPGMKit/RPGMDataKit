using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeActorImagesCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeActorImages;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Actor Images";
        else
            return "Change Actor Graphic";
    }

    public int ActorId { get; set; }
    public string CharacterImage { get; set; }
    public int CharacterImageOffset { get; set; }
    public string FaceImage { get; set; }
    public int FaceImageOffset { get; set; }
    public string SwBattler { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ActorId = parameterProvider.GetParameter<int>(0);
        CharacterImage = parameterProvider.GetParameter<string>(1);
        CharacterImageOffset = parameterProvider.GetParameter<int>(2);
        FaceImage = parameterProvider.GetParameter<string>(3);
        FaceImageOffset = parameterProvider.GetParameter<int>(4);
        if (version == StorageVersion.MV)
            SwBattler = parameterProvider.GetParameter<string>(5);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ActorId);
        parameterProvider.SetParameter(1, CharacterImage);
        parameterProvider.SetParameter(2, CharacterImageOffset);
        parameterProvider.SetParameter(3, FaceImage);
        parameterProvider.SetParameter(4, FaceImageOffset);
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(5, SwBattler);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatActor(ActorId);

        var format = context.Version == StorageVersion.MV ? ", {0}({1})" : ", '{0}', {1}";
        var none = context.Version == StorageVersion.MV ? ", None" : ", (None)";

        if (!string.IsNullOrEmpty(FaceImage))
            str += string.Format(format, FaceImage, FaceImageOffset);
        else
            str += none;

        if (!string.IsNullOrEmpty(CharacterImage))
            str += string.Format(format, CharacterImage, CharacterImageOffset);
        else
            str += none;

        if (context.Version == StorageVersion.MV)
        {
            if (!string.IsNullOrEmpty(SwBattler))
                str += $", {SwBattler}";
            else
                str += none;
        }

        return str;
    }
}