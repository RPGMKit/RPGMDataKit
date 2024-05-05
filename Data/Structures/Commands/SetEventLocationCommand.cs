using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetEventLocationCommand : NamedCommand
    {
        protected const int TRANSFER_DIRECT = 0;
        protected const int TRANSFER_BY_VARIABLE = 1;
        protected const int TRANSFER_TYPE_EXCHANGE = 2;

        public int GetEvent()
        {
            return GetParameter<int>(0);
        }

        public int GetTransferType()
        {
            return GetParameter<int>(1);
        }

        public int GetX()
        {
            return GetParameter<int>(2);
        }

        public int GetY()
        {
            return GetParameter<int>(3);
        }

        public int GetDirection()
        {
            return GetParameter<int>(4);
        }

        public override int Code => CommandCodes.SetEventLocation;
        public override string Name =>"Set Event Location";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            if (GetEvent() == 0)
                str = "This event";
            else
                str = $"#{GetEvent()}";

            str += ", ";

            switch (GetTransferType())
            {
                case TRANSFER_DIRECT:
                    str += $"({GetX()},{GetY()}";
                    break;
                case TRANSFER_BY_VARIABLE:
                    str += "(";
                    str += "{#" + GetX();
                    var variableName = storage.GetVariable(GetX());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "},";
                    str += "{#" + GetY();
                    variableName = storage.GetVariable(GetY());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "})";
                    break;
                case TRANSFER_TYPE_EXCHANGE:
                    str += "Exchange with #{GetX()}";
                    break;
                default:
                    throw new Exception($"Unknown set event location transfer type '{GetTransferType()}'");
            }

            str += $" (Direction: {GetDirection()}";

            return str;
        }
    }
}
