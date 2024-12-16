using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeVehicleBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeVehicleBGM;
    public override string GetCommandName(StorageVersion version) => "Change Vehicle BGM";

    public VehicleType VehicleType { get; set; }
    public IAudioFile SoundData { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        VehicleType = parameterProvider.GetParameter<VehicleType>(0);
        SoundData = version == StorageVersion.MV ? parameterProvider.GetParameter<SoundData>(1) : throw new Exception("Exists only in MV");
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, VehicleType);
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(1, SoundData);
        else
            throw new Exception("Exists only in MV");
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = VehicleType switch
        {
            VehicleType.Boat => "Boat",
            VehicleType.Ship => "Ship",
            VehicleType.Airship => "Airship",
            _ => throw new Exception($"Unknown vehicle type: {VehicleType}")
        };
        
        return str + ", " + context.NameFormatter.FormatSoundData(SoundData);
    }
}
