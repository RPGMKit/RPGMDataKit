namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfActorCommand : IfCommand
    {
        protected const int CHECK_IN_PARTY = 0;
        protected const int CHECK_NAME = 1;
        protected const int CHECK_CLASS = 2;
        protected const int CHECK_SKILL = 3;
        protected const int CHECK_WEAPON_EQUIPPED = 4;
        protected const int CHECK_ARMOR_EQUIPPED = 5;
        protected const int CHECK_STATE = 6;

        public int GetUnitId()
        {
            return (int)GetParameter(1);
        }

        public int GetCheckType()
        {
            return (int)GetParameter(2);
        }

        public object GetCheckParameter()
        {
            return GetParameter(3);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"Unit #{GetUnitId()}";

            switch (GetCheckType())
            {
                case CHECK_IN_PARTY:
                    str += " is in the party";
                    break;
                case CHECK_NAME:
                    str += $" name is '{GetCheckParameter()}'";
                    break;
                case CHECK_CLASS:
                    str += $" class is #{GetCheckParameter()}";
                    break;
                case CHECK_SKILL:
                    str += $" has learned #{GetCheckParameter()}";
                    break;
                case CHECK_WEAPON_EQUIPPED:
                    str += $" has equipped weapon #{GetCheckParameter()}";
                    break;
                case CHECK_ARMOR_EQUIPPED:
                    str += $" has equipped armor #{GetCheckParameter()}";
                    break;
                case CHECK_STATE:
                    str += $" is affected by #{GetCheckParameter()} state";
                    break;
                default:
                    throw new Exception($"Unknown check type '{GetCheckType()}' for '{Name}' actor command");
            }

            return str;
        }
    }
}
