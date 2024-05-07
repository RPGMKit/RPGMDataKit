using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetVehicleLocationCommand : NamedCommand
    {
        protected const int TRANSFER_DIRECT = 0;
        protected const int TRANSFER_BY_VARIABLE = 1;

        public int GetVehicle()
        {
            return GetParameter<int>(0);
        }

        public int GetTransferType()
        {
            return GetParameter<int>(1);
        }

        public int GetMap()
        {
            return GetParameter<int>(2);
        }

        public int GetX()
        {
            return GetParameter<int>(3);
        }

        public int GetY()
        {
            return GetParameter<int>(4);
        }

        public override int Code => CommandCodes.SetVehicleLocation;
        public override string GetCommandName(StorageVersion version) => "Set Vehicle Location";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = $"#{GetVehicle()}, ";

            switch (GetTransferType())
            {
                case TRANSFER_DIRECT:
                    str += $"#{GetMap()} ";
                    var mapInfo = context.Storage.GetMapInfo(GetMap());
                    if (mapInfo != null)
                        str += $" '{mapInfo.Name}' ";
                    str += $"({GetX()},{GetY()}";
                    break;
                case TRANSFER_BY_VARIABLE:
                    str += "{#" + GetMap();
                    var variableName = context.Storage.GetVariable(GetMap());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "} (";
                    str += "{#" + GetX();
                    variableName = context.Storage.GetVariable(GetX());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "},";
                    str += "{#" + GetY();
                    variableName = context.Storage.GetVariable(GetY());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "})";
                    break;
                default:
                    throw new Exception($"Unknown set vehicle location transfer type '{GetTransferType()}'");
            }

            return str;
        }
    }
}
