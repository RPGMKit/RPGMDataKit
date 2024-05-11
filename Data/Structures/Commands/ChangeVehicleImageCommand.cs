using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeVehicleImageCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeVehicleImage;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Vehicle Image";
        else
            return "Change Vehicle Graphic";
    }

    public VehicleType GetVehicleType()
    {
        return (VehicleType)GetParameter<int>(0);
    }

    public string GetImage()
    {
        return GetParameter<string>(1);
    }

    public int GetImageOffset()
    {
        return GetParameter<int>(2);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = GetVehicleType() switch
        {
            VehicleType.Boat => "Boat",
            VehicleType.Ship => "Ship",
            VehicleType.Airship => "Airship",
            _ => throw new Exception($"Unknown vehicle type: {GetVehicleType()}")
        };
            
        var format = context.Version == StorageVersion.MV ? ", {0}({1})" : ", '{0}', {1}";
        var none = context.Version == StorageVersion.MV ? ", None" : ", (None)";
            
        var image = GetImage();
        if (!string.IsNullOrEmpty(image))
            str += string.Format(format, image, GetImageOffset());
        else
            str += none;

        return str;
    }
}