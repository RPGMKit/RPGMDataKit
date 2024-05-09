using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeVehicleBGMCommand : AbstractAudioCommand
{
    protected const int VehicleTypeBoat = 0;
    protected const int VehicleTypeShip = 1;
    protected const int VehicleTypeAirship = 2;
    
    public override int Code => CommandCodes.ChangeVehicleBGM;

    public override string GetCommandName(StorageVersion version) => "Change Vehicle BGM";

    public int GetVehicle()
    {
        return GetParameter<int>(0);
    }
    
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        string str = GetVehicle() switch
        {
            VehicleTypeBoat => "Boat",
            VehicleTypeShip => "Ship",
            VehicleTypeAirship => "Airship",
            _ => throw new Exception($"Unknown vehicle type: {GetVehicle()}")
        };
        
        return str + ", " + FormatSoundData(1, context, options);
    }
}
