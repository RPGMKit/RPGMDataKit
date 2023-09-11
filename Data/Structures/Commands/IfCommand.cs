using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public abstract class IfCommand : NamedCommand
    {
        protected const int TYPE_SWITCH = 0;
        protected const int TYPE_VARIABLE = 1;
        protected const int TYPE_SELF_SWITCH = 2;
        protected const int TYPE_TIMER = 3;
        protected const int TYPE_ACTOR = 4;
        protected const int TYPE_ENEMY = 5;
        protected const int TYPE_CHARACTER = 6;
        protected const int TYPE_GOLD = 7;
        protected const int TYPE_ITEM = 8;
        protected const int TYPE_WEAPON = 9;
        protected const int TYPE_ARMOR = 10;
        protected const int TYPE_BUTTON = 11;
        protected const int TYPE_SCRIPT = 12;
        protected const int TYPE_VEHICLE = 13;

        public static NamedCommand IfFactory(LookupList<JToken> parameters)
        {
            var ifType = (int)parameters[0];

            switch (ifType)
            {
                case TYPE_SWITCH:
                    return new IfSwitchCommand();
                case TYPE_VARIABLE:
                    return new IfVariableCommand();
                case TYPE_SELF_SWITCH:
                    return new IfSelfSwitchCommand();
                case TYPE_TIMER:
                    return new IfTimerCommand();
                case TYPE_ACTOR:
                    return new IfActorCommand();
                case TYPE_ENEMY:
                    return new IfEnemyCommand();
                case TYPE_CHARACTER:
                    return new IfCharacterCommand();
                case TYPE_GOLD:
                    return new IfGoldCommand();
                case TYPE_ITEM:
                    return new IfItemCommand();
                case TYPE_WEAPON:
                    return new IfWeaponCommand();
                case TYPE_ARMOR:
                    return new IfArmorCommand();
                case TYPE_BUTTON:
                    return new IfButtonCommand();
                case TYPE_SCRIPT:
                    return new IfScriptCommand();
                case TYPE_VEHICLE:
                    return new IfVehicleCommand();
                default:
                    throw new Exception($"Unknown if type '{ifType}' command");

            }
        }

        public int GetIfType()
        {
            return (int)GetParameter(0);
        }

        public override int Code => CommandCodes.If;
        public override string Name => "If";
    }
}
