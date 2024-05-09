using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetVehicleLocationCommand : NamedCommand
    {
        protected const int TRANSFER_DIRECT = 0;
        protected const int TRANSFER_BY_VARIABLE = 1;

        protected const int VehicleTypeBoat = 0;
        protected const int VehicleTypeShip = 1;
        protected const int VehicleTypeAirship = 2;

        public int GetVehicle()
        {
            return GetParameter<int>(0);
        }

        public int GetDesignationType()
        {
            return GetParameter<int>(1);
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
            string str = GetVehicle() switch
            {
                VehicleTypeBoat => "Boat",
                VehicleTypeShip => "Ship",
                VehicleTypeAirship => "Airship",
                _ => throw new Exception($"Unknown vehicle type: {GetVehicle()}")
            };

            switch (GetDesignationType())
            {
                case TRANSFER_DIRECT:
                    str += ", " + context.NameFormatter.FormatMap(GetParam1());

                    if (context.Version == StorageVersion.MV)
                        str += $" ({GetParam2()},{GetParam3()})";
                    else
                        str += $" ({GetParam2():D3},{GetParam3():D3})";
                    break;
                case TRANSFER_BY_VARIABLE:
                    string var1, var2, var3;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1(),
                            new() { BracketType = BracketType.Figure });
                        var2 = context.NameFormatter.FormatVariable(GetParam2(),
                            new() { BracketType = BracketType.Figure });
                        var3 = context.NameFormatter.FormatVariable(GetParam3(),
                            new() { BracketType = BracketType.Figure });

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
