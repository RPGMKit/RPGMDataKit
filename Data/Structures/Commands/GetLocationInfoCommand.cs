using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class GetLocationInfoCommand : NamedCommand
    {
        protected const int DESIGNATION_TYPE_DIRECT = 0;
        protected const int DESIGNATION_TYPE_BY_VARIABLES = 1;

        public int GetVariable()
        {
            return GetParameter<int>(0);
        }

        public int GetInfoType()
        {
            return GetParameter<int>(1);
        }

        public int GetDesignationType()
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

        public override int Code => CommandCodes.GetLocationInfo;
        public override string GetCommandName(StorageVersion version) => "Get Location Info";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetVariable()}";
            var variableName = storage.GetVariable(GetVariable());
            if (!string.IsNullOrEmpty(variableName))
                str += $" {variableName}";

            str += $", Type {GetInfoType()}, ";

            switch (GetDesignationType())
            {
                case DESIGNATION_TYPE_DIRECT:
                    str += $"({GetX()},{GetY()})";
                    break;
                case DESIGNATION_TYPE_BY_VARIABLES:
                    str += "({#" + GetX();
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
                    throw new Exception($"Unknown get location info designation type '{GetDesignationType()}'");
            }

            return str;
        }
    }
}

