using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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
    
    public VehicleType VehicleType
    {
        get => Get<VehicleType>(0);
        set => Set(0, value);
    }

    [EditorDefault("")]
    public string Image
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public int Offset
    {
        get => Get<int>(2);
        set => Set(2, value);
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

        var format = context.Version == StorageVersion.MV ? ", {0}({1})" : ", '{0}', {1}";
        var none = context.Version == StorageVersion.MV ? ", None" : ", (None)";

        if (!string.IsNullOrEmpty(Image))
            str += string.Format(format, Image, Offset);
        else
            str += none;

        return str;
    }
}