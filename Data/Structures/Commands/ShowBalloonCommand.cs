using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowBalloonCommand : NamedCommand
    {
        public enum IconType
        {
            Exclamation = 1,
            Question = 2,
            MusicNote = 3,
            Heart = 4,
            Anger = 5,
            Sweat = 6,
            Cobweb = 7,
            Silence = 8,
            LightBulb = 9,
            Zzz = 10,
            UserDefined1 = 11,
            UserDefined2 = 12,
            UserDefined3 = 13,
            UserDefined4 = 14,
            UserDefined5 = 15,
        }

        public override int Code => CommandCodes.ShowBaloonIcon;
        public override string GetCommandName(StorageVersion version) => "Show Balloon Icon";

        public int GetSource()
        {
            return GetParameter<int>(0);
        }

        public IconType GetBalloonIcon()
        {
            return (IconType)GetParameter<int>(1);
        }

        public bool GetWaitForCompletion()
        {
            return GetParameter<bool>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatEvent(GetSource());

            str += ", ";

            switch (GetBalloonIcon())
            {
                case IconType.Exclamation:
                    str += "Exclamation";
                    break;
                case IconType.Question:
                    str += "Question";
                    break;
                case IconType.MusicNote:
                    str += "Music Note";
                    break;
                case IconType.Heart:
                    str += "Heart";
                    break;
                case IconType.Anger:
                    str += "Anger";
                    break;
                case IconType.Sweat:
                    str += "Sweat";
                    break;
                case IconType.Cobweb:
                    str += "Cobweb";
                    break;
                case IconType.Silence:
                    str += "Silence";
                    break;
                case IconType.LightBulb:
                    str += "Light Bulb";
                    break;
                case IconType.Zzz:
                    str += "Zzz";
                    break;
                case IconType.UserDefined1:
                    str += "User-defined 1";
                    break;
                case IconType.UserDefined2:
                    str += "User-defined 2";
                    break;
                case IconType.UserDefined3:
                    str += "User-defined 3";
                    break;
                case IconType.UserDefined4:
                    str += "User-defined 4";
                    break;
                case IconType.UserDefined5:
                    str += "User-defined 5";
                    break;
                default:
                    throw new Exception($"Unknown balloon icon '{GetBalloonIcon()}'");
            }

            if (GetWaitForCompletion())
                str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

            return str;
        }
    }
}
