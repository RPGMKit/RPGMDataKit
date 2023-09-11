using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowChoicesCommand : NamedCommand
    {
        protected const int BACKGROUND_WINDOW = 0;
        protected const int BACKGROUND_DIM = 1;
        protected const int BACKGROUND_TRANSPARENT = 2;
        protected const int POSITION_LEFT = 0;
        protected const int POSITION_RIGHT = 2;
        protected const int POSITION_MIDDLE = 1;
        protected const int DEFAULT_CHOICE_NONE = -1;
        protected const int CANCEL_CHOICE_BRANCH = -2;
        protected const int CANCEL_CHOICE_DISALLOW = -1;

        public override int Code => CommandCodes.ShowChoices;
        public override string Name => "Show Choices";

        public List<string> GetChoices()
        {
            return GetParameter(0).ToObject<List<string>>();
        }

        public int GetCancelChoice()
        {
            return (int)GetParameter(1);
        }

        public int GetDefaultChoice()
        {
            return (int)GetParameter(2);
        }

        public int GetPosition()
        {
            return (int)GetParameter(3);
        }

        public int GetBackground()
        {
            return (int)GetParameter(4);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var background = "";
            switch (GetBackground())
            {
                case BACKGROUND_WINDOW:
                    background = "Window";
                    break;
                case BACKGROUND_DIM:
                    background = "Dim";
                    break;
                case BACKGROUND_TRANSPARENT:
                    background = "Transparent";
                    break;
                default:
                    throw new Exception($"Unknown background '{GetBackground()}'");
            }

            var position = "";
            switch (GetPosition())
            {
                case POSITION_RIGHT:
                    position = "Right";
                    break;
                case POSITION_MIDDLE:
                    position = "Middle";
                    break;
                case POSITION_LEFT:
                    position = "Left";
                    break;
                default:
                    throw new Exception($"Unknown position '{GetPosition()}'");
            }

            var defaultChoice = "";
            switch (GetDefaultChoice())
            {
                case DEFAULT_CHOICE_NONE:
                    defaultChoice = "-";
                    break;
                default:
                    defaultChoice = "#" + (GetDefaultChoice() + 1);
                    break;
            }

            var cancelChoice = "";
            switch (GetCancelChoice())
            {
                case CANCEL_CHOICE_BRANCH:
                case CANCEL_CHOICE_DISALLOW:
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
