using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetVehicleLocationCommand : NamedCommand
    {
        public enum DesignationType
        {
            Direct = 0,
            ByVariable = 1,
        }
        
        public VehicleType GetVehicleType()
        {
            return (VehicleType)GetParameter<int>(0);
        }

        public DesignationType GetDesignationType()
        {
            return (DesignationType)GetParameter<int>(1);
        }

        public int GetParam1()
        {
            return GetParameter<int>(2);
        }

        public int GetParam2()
        {
            return GetParameter<int>(3);
        }

        public int GetParam3()
        {
            return GetParameter<int>(4);
        }

        public override int Code => CommandCodes.SetVehicleLocation;
        public override string GetCommandName(StorageVersion version) => "Set Vehicle Location";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            string str = GetVehicleType() switch
            {
                VehicleType.Boat => "Boat",
                VehicleType.Ship => "Ship",
                VehicleType.Airship => "Airship",
                _ => throw new Exception($"Unknown vehicle type: {GetVehicleType()}")
            };

            switch (GetDesignationType())
            {
                case DesignationType.Direct:
                    str += ", " + context.NameFormatter.FormatMap(GetParam1());

                    if (context.Version == StorageVersion.MV)
                        str += $" ({GetParam2()},{GetParam3()})";
                    else
                        str += $" ({GetParam2():D3},{GetParam3():D3})";
                    break;
                case DesignationType.ByVariable:
                    string var1, var2, var3;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1());
                        var2 = context.NameFormatter.FormatVariable(GetParam2());
                        var3 = context.NameFormatter.FormatVariable(GetParam3());

                        str += $", {var1} ({var2},{var3})";
                    }
                    else
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1(), new()
                        {
                            SkipTitle = true,
                            SkipName = true,
                        });
                        var2 = context.NameFormatter.FormatVariable(GetParam2(), new()
                        {
                            SkipTitle = true,
                            SkipName = true,
                        });
                        var3 = context.NameFormatter.FormatVariable(GetParam3(), new()
                        {
                            SkipTitle = true,
                            SkipName = true,
                        });

                        str += $", Variable {var1}{var2}{var3}";
                    }

                    break;
                default:
                    throw new Exception($"Unknown set vehicle location transfer type '{GetDesignationType()}'");
            }

            return str;
        }
    }
}
