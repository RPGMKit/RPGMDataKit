using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfVehicleCommand : IfCommand
    {
        protected const int VehicleTypeBoat = 0;
        protected const int VehicleTypeShip = 1;
        protected const int VehicleTypeAirship = 2;
        
        public int GetVehicle()
        {
            return GetParameter<int>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = GetVehicle() switch
            {
                VehicleTypeBoat => "Boat",
                VehicleTypeShip => "Ship",
                VehicleTypeAirship => "Airship",
                _ => throw new Exception($"Unknown vehicle type: {GetVehicle()}")
            };

            if (context.Version == StorageVersion.MV)
                return str + " is driven";
            else
                return str + " is Driven";
        }
    }
}
