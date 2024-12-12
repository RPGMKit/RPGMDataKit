using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeDefeatME : NamedCommand
{
    public override int Code => CommandCodes.ChangeDefeatME;

    public override string GetCommandName(StorageVersion version) => "Change Defeat ME";
    
    public IAudioFile SoundData { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        SoundData = version == StorageVersion.MV ? parameterProvider.GetParameter<SoundData>(0) : throw new Exception("Exists only in MV");
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(0, SoundData);
        else
            throw new Exception("Exists only in MV");
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}
