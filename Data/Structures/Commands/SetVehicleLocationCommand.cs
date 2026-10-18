using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class SetVehicleLocationCommand : NamedCommand
{
    public override int Code => CommandCodes.SetVehicleLocation;
    public override string GetCommandName(StorageVersion version) => "Set Vehicle Location";
    
    public enum DesignationType
    {
        Direct = 0,
        ByVariable = 1,
    }
        
    public VehicleType VehicleType
    {
        get => Get<VehicleType>(0);
        set => Set(0, value);
    }

    public DesignationType Designation
    {
        get => Get<DesignationType>(1);
        set => Set(1, value);
    }

    [EditorDefault(1, StorageVersion.VXAce)]
    public int Param1
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public int Param2
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public int Param3
    {
        get => Get<int>(4);
        set => Set(4, value);
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