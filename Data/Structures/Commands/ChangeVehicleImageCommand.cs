using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
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
    
    public VehicleType VehicleType { get; set; }
    public string Image { get; set; }
    public int Offset { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        VehicleType = (VehicleType)parameterProvider.GetParameter<int>(0);
        Image = parameterProvider.GetParameter<string>(1);
        Offset = parameterProvider.GetParameter<int>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, (int)VehicleType);
        parameterProvider.SetParameter(1, Image);
        parameterProvider.SetParameter(2, Offset);
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