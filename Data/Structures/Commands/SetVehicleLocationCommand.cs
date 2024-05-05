using System;
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
        public override string Name => "Set Vehicle Location";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetVehicle()}, ";

            switch (GetTransferType())
            {
                case TRANSFER_DIRECT:
                    str += $"#{GetMap()} ";
                    var mapInfo = storage.GetMapInfo(GetMap());
                    if (mapInfo != null)
                        str += $" '{mapInfo.Name}' ";
                    str += $"({GetX()},{GetY()}";
                    break;
                case TRANSFER_BY_VARIABLE:
                    str += "{#" + GetMap();
                    var variableName = storage.GetVariable(GetMap());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "} (";
                    str += "{#" + GetX();
                    variableName = storage.GetVariable(GetX());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "},";
                    str += "{#" + GetY();
                    variableName = storage.GetVariable(GetY());
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
