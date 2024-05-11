using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfVehicleCommand : IfCommand
    {
        public VehicleType GetVehicle()
        {
            return (VehicleType)GetParameter<int>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = GetVehicle() switch
            {
                VehicleType.Boat => "Boat",
                VehicleType.Ship => "Ship",
                VehicleType.Airship => "Airship",
                _ => throw new Exception($"Unknown vehicle type: {GetVehicle()}")
            };

            if (context.Version == StorageVersion.MV)
                return str + " is driven";
            else
                return str + " is Driven";
        }
    }
}
