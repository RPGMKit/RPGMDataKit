namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfCharacterCommand : IfCommand
    {
        protected const int ORIENTATION_DOWN = 2;
        protected const int ORIENTATION_LEFT = 4;
        protected const int ORIENTATION_RIGHT = 6;
        protected const int ORIENTATION_UP = 8;

        public int GetSourceType()
        {
            return GetParameter<int>(1);
        }

        public int GetOrientation()
        {
            return GetParameter<int>(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            switch (GetSourceType())
            {
                case -1:
                    str = "Player";
                    break;
                case 0:
                    str = "This Event";
                    break;
                default:
                    str = $"Event #{GetSourceType()}";
                    break;
            }

            str += " is facing ";
            switch (GetOrientation())
            {
                case ORIENTATION_DOWN:
                    str += "Down";
                    break;
                case ORIENTATION_LEFT:
                    str += "Left";
                    break;
                case ORIENTATION_RIGHT:
                    str += "Right";
                    break;
                case ORIENTATION_UP:
                    str += "Up";
                    break;
                default:
                    throw new Exception(
                        $"Unknown orientation '{GetOrientation()}' for '{Name}' orientation command");
            }

            return str;
        }
    }
}
