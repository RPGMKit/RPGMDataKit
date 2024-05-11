using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowChoicesCommand : NamedCommand
    {
        public enum BackgroundType
        {
            Window = 0,
            Dim = 1,
            Transparent = 2,
        }

        public enum PositionType
        {
            Left = 0,
            Middle = 1,
            Right = 2,
        }

        protected const int DefaultChoiceNone = -1;
        protected const int CancelChoiceBranch = -2;
        protected const int CancelChoiceDisallow = -1;

        public override int Code => CommandCodes.ShowChoices;
        public override string GetCommandName(StorageVersion version) => "Show Choices";

        public List<string> GetChoices()
        {
            return GetParameter<List<string>>(0);
        }

        public int GetCancelChoice()
        {
            return GetParameter<int>(1);
        }

        public int GetDefaultChoice()
        {
            return GetParameter<int>(2);
        }

        public PositionType GetPosition()
        {
            return (PositionType)GetParameter<int>(3);
        }

        public BackgroundType GetBackground()
        {
            return (BackgroundType)GetParameter<int>(4);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var background = "";
            switch (GetBackground())
            {
                case BackgroundType.Window:
                    background = "Window";
                    break;
                case BackgroundType.Dim:
                    background = "Dim";
                    break;
                case BackgroundType.Transparent:
                    background = "Transparent";
                    break;
                default:
                    throw new Exception($"Unknown background '{GetBackground()}'");
            }

            var position = "";
            switch (GetPosition())
            {
                case PositionType.Right:
                    position = "Right";
                    break;
                case PositionType.Middle:
                    position = "Middle";
                    break;
                case PositionType.Left:
                    position = "Left";
                    break;
                default:
                    throw new Exception($"Unknown position '{GetPosition()}'");
            }

            var defaultChoice = "";
            switch (GetDefaultChoice())
            {
                case DefaultChoiceNone:
                    defaultChoice = "-";
                    break;
                default:
                    defaultChoice = "#" + (GetDefaultChoice() + 1);
                    break;
            }

            var cancelChoice = "";
            switch (GetCancelChoice())
            {
                case CancelChoiceBranch:
                case CancelChoiceDisallow:
                    cancelChoice = "-";
                    break;
                default:
                    cancelChoice = "#" + (GetCancelChoice() + 1);
                    break;
            }

            return string.Join(", ", GetChoices()) + $", ({background}, {position}, {defaultChoice}, {cancelChoice})";
        }
    }
}
