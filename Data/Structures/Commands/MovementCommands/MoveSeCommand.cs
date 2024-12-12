using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveSeCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Se;
    public override string GetCommandName(StorageVersion version) => "SE";

    public IAudioFile SoundData { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        SoundData = version == StorageVersion.MV ? parameterProvider.GetParameter<SoundData>(0) : parameterProvider.GetParameter<IAudioFile>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(0, SoundData);
        else
            parameterProvider.SetParameter(0,
                new SE() { Name = SoundData.Name, Volume = SoundData.Volume, Pitch = SoundData.Pitch });
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}