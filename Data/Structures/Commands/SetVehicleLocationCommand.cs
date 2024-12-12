using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class SetVehicleLocationCommand : NamedCommand
{
    public override int Code => CommandCodes.SetVehicleLocation;
    public override string GetCommandName(StorageVersion version) => "Set Vehicle Location";
    
    public enum DesignationType
    {
        Direct = 0,
        ByVariable = 1,
    }
        
    public VehicleType VehicleType { get; set; }
    public DesignationType Designation { get; set; }
    public int Param1 { get; set; }
    public int Param2 { get; set; }
    public int Param3 { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        VehicleType = (VehicleType)parameterProvider.GetParameter<int>(0);
        Designation = (DesignationType)parameterProvider.GetParameter<int>(1);
        Param1 = parameterProvider.GetParameter<int>(2);
        Param2 = parameterProvider.GetParameter<int>(3);    
        Param3 = parameterProvider.GetParameter<int>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, (int)VehicleType);
        parameterProvider.SetParameter(1, (int)Designation);
        parameterProvider.SetParameter(2, Param1);
        parameterProvider.SetParameter(3, Param2);
        parameterProvider.SetParameter(4, Param3);
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

        switch (Designation)
        {
            case DesignationType.Direct:
                str += ", " + context.NameFormatter.FormatMap(Param1);

                if (context.Version == StorageVersion.MV)
                    str += $" ({Param2},{Param3})";
                else
                    str += $" ({Param2:D3},{Param3:D3})";
                break;
            case DesignationType.ByVariable:
                string var1, var2, var3;
                if (context.Version == StorageVersion.MV)
                {
                    var1 = context.NameFormatter.FormatVariable(Param1);
                    var2 = context.NameFormatter.FormatVariable(Param2);
                    var3 = context.NameFormatter.FormatVariable(Param3);

                    str += $", {var1} ({var2},{var3})";
                }
                else
                {
                    var1 = context.NameFormatter.FormatVariable(Param1, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });
                    var2 = context.NameFormatter.FormatVariable(Param2, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });
                    var3 = context.NameFormatter.FormatVariable(Param3, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });

                    str += $", Variable {var1}{var2}{var3}";
                }

                break;
            default:
                throw new Exception($"Unknown set vehicle location transfer type '{Designation}'");
        }

        return str;
    }
}