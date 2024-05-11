using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeVehicleBGMCommand : AbstractAudioCommand
{
    public override int Code => CommandCodes.ChangeVehicleBGM;

    public override string GetCommandName(StorageVersion version) => "Change Vehicle BGM";

    public VehicleType GetVehicleType()
    {
        return (VehicleType)GetParameter<int>(0);
    }
    
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        string str = GetVehicleType() switch
        {
            VehicleType.Boat => "Boat",
            VehicleType.Ship => "Ship",
            VehicleType.Airship => "Airship",
            _ => throw new Exception($"Unknown vehicle type: {GetVehicleType()}")
        };
        
        return str + ", " + FormatSoundData(1, context, options);
    }
}
