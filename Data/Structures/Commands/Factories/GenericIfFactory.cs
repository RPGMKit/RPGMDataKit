using RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories;

public static class GenericIfFactory
{
    public const int TYPE_SWITCH = 0;
    public const int TYPE_VARIABLE = 1;
    public const int TYPE_SELF_SWITCH = 2;
    public const int TYPE_TIMER = 3;
    public const int TYPE_ACTOR = 4;
    public const int TYPE_ENEMY = 5;
    public const int TYPE_CHARACTER = 6;
    public const int TYPE_GOLD = 7;
    public const int TYPE_ITEM = 8;
    public const int TYPE_WEAPON = 9;
    public const int TYPE_ARMOR = 10;
    public const int TYPE_BUTTON = 11;
    public const int TYPE_SCRIPT = 12;
    public const int TYPE_VEHICLE = 13;

    public static IfCommand Create(int ifType)
    {
        switch (ifType)
        {
            case TYPE_SWITCH:
                return new IfSwitchCommand();
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
}