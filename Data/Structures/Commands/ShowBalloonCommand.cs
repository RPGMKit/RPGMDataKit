using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowBalloonCommand : NamedCommand
    {
        protected const int SOURCE_PLAYER = -1;
        protected const int SOURCE_THIS_EVENT = 0;
        protected const int ICON_EXCLAMATION = 1;
        protected const int ICON_QUESTION = 2;
        protected const int ICON_MUSIC_NOTE = 3;
        protected const int ICON_HEART = 4;
        protected const int ICON_ANGER = 5;
        protected const int ICON_SWEAT = 6;
        protected const int ICON_COBWEB = 7;
        protected const int ICON_SILENCE = 8;
        protected const int ICON_LIGHT_BULB = 9;
        protected const int ICON_ZZZ = 10;
        protected const int ICON_USER_DEFINED_1 = 11;
        protected const int ICON_USER_DEFINED_2 = 12;
        protected const int ICON_USER_DEFINED_3 = 13;
        protected const int ICON_USER_DEFINED_4 = 14;
        protected const int ICON_USER_DEFINED_5 = 15;

        public override int Code => CommandCodes.ShowBaloonIcon;
        public override string Name => "Show Balloon Icon";

        public int GetSource()
        {
            return (int)GetParameter(0);
        }

        public int GetBalloonIcon()
        {
            return (int)GetParameter(1);
        }

        public bool GetWaitForCompletion()
        {
            return (bool)GetParameter(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            switch (GetSource())
            {
                case SOURCE_PLAYER:
                    str = "Player";
                    break;
                case SOURCE_THIS_EVENT:
                    str = "This Event";
                    break;
                default:
                    str = $"Event #{GetSource()}";
                    break;
            }

            str += ", ";

            switch (GetBalloonIcon())
            {
                case ICON_EXCLAMATION:
                    str += "Exclamation";
                    break;
                case ICON_QUESTION:
                    str += "Question";
                    break;
                case ICON_MUSIC_NOTE:
                    str += "Music Note";
                    break;
                case ICON_HEART:
                    str += "Heart";
                    break;
                case ICON_ANGER:
                    str += "Anger";
                    break;
                case ICON_SWEAT:
                    str += "Sweat";
                    break;
                case ICON_COBWEB:
                    str += "Cobweb";
                    break;
                case ICON_SILENCE:
                    str += "Silence";
                    break;
                case ICON_LIGHT_BULB:
                    str += "Light Bulb";
                    break;
                case ICON_ZZZ:
                    str += "Zzz";
                    break;
                case ICON_USER_DEFINED_1:
                    str += "User-defined 1";
                    break;
                case ICON_USER_DEFINED_2:
                    str += "User-defined 2";
                    break;
                case ICON_USER_DEFINED_3:
                    str += "User-defined 3";
                    break;
                case ICON_USER_DEFINED_4:
                    str += "User-defined 4";
                    break;
                case ICON_USER_DEFINED_5:
                    str += "User-defined 5";
                    break;
                default:
                    throw new Exception($"Unknown balloon icon '{GetBalloonIcon()}'");
            }

            if (GetWaitForCompletion())
            {
                str += " (Wait)";
            }

            return str;
        }
    }
}
