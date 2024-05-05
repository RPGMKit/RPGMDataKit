using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class TransferPlayerCommand : NamedCommand
    {
        protected const int TRANSFER_DIRECT = 0;
        protected const int TRANSFER_BY_VARIABLE = 1;
        protected const int DIRECTION_RETAIN = 0;
        protected const int DIRECTION_DOWN = 2;
        protected const int DIRECTION_LEFT = 4;
        protected const int DIRECTION_RIGHT = 6;
        protected const int DIRECTION_UP = 8;
        protected const int FADE_BLACK = 0;
        protected const int FADE_WHITE = 1;
        protected const int FADE_NONE = 2;

        public int GetTransferType()
        {
            return GetParameter<int>(0);
        }

        public int GetMap()
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

        public int GetFade()
        {
            return GetParameter<int>(5);
        }

        public override int Code => CommandCodes.TransferPlayer;
        public override string GetCommandName(StorageVersion version) => "Transfer player";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            switch (GetTransferType())
            {
                case TRANSFER_DIRECT:
                    str = $"#{GetMap()} ";
                    var mapInfo = storage.GetMapInfo(GetMap());
                    if (mapInfo != null)
                        str += $" '{mapInfo.Name}' ";

                    str += $"({GetX()},{GetY()})";
                    break;
                case TRANSFER_BY_VARIABLE:
                    str = "{#" + GetMap();
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
                    throw new Exception($"Unknown transfer player transfer type '{GetTransferType()}'");
            }

            var parts = new List<string>();
            if (GetDirection() != DIRECTION_RETAIN)
            {
                var direction = "Direction: ";
                switch (GetDirection())
                {
                    case DIRECTION_DOWN:
                        direction += "Down";
                        break;
                    case DIRECTION_LEFT:
                        direction += "Left";
                        break;
                    case DIRECTION_RIGHT:
                        direction += "Right";
                        break;
                    case DIRECTION_UP:
                        direction += "Up";
                        break;
                    default:
                        throw new Exception($"Unknown direction '{GetDirection()}'");
                }

                parts.Add(direction);
            }

            if (GetFade() != FADE_BLACK)
            {
                var fade = "Fade: ";
                switch (GetFade())
                {
                    case FADE_WHITE:
                        fade += "White";
                        break;
                    case FADE_NONE:
                        fade += "None";
                        break;
                    default:
                        throw new Exception($"Unknown fade '{GetFade()}'");
                }

                parts.Add(fade);
            }

            if (parts.Count > 0)
            {
                str += " (" + string.Join(", ", parts) + ")";
            }

            return str;
        }
    }
}
