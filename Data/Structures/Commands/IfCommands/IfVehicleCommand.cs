using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfVehicleCommand : IfCommand
{
    public VehicleType VehicleType
    {
        get => Get<VehicleType>(1);
        set => Set(1, value);
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

        if (context.Version == StorageVersion.MV)
            return str + " is driven";
        else
            return str + " is Driven";
    }
}