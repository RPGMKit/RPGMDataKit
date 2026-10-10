using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public abstract class IfCommand : NamedCommand
{
    public override int Code => CommandCodes.If;

    public override string GetCommandName(StorageVersion version)
    {
        return version == StorageVersion.MV ? "If" : "Conditional Branch";
    }

    public enum IfType
    {
        Switch = 0,
        Variable = 1,
        SelfSwitch = 2,
        Timer = 3,
        Actor = 4,
        Enemy = 5,
        Event = 6,
        Gold = 7,
        Item = 8,
        Weapon = 9,
        Armor = 10,
        Button = 11,
        Script = 12,
        Vehicle = 13,
    }

    private static readonly Dictionary<IfType, Type> Kinds =
        Enum.GetValues<IfType>().ToDictionary(type => type, type => Factory(type).GetType());

    public IfType Type
    {
        get => Get<IfType>(0);
        set => Set(0, value);
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        Type = Kinds.First(kind => kind.Value == GetType()).Key;
    }

    public static IfCommand Factory(IfType type)
    {
        switch (type)
        {
            case IfType.Variable:
                return new IfVariableCommand();
            case IfType.Switch:
                return new IfSwitchCommand();
            case IfType.SelfSwitch:
                return new IfSelfSwitchCommand();
            case IfType.Timer:
                return new IfTimerCommand();
            case IfType.Actor:
                return new IfActorCommand();
            case IfType.Enemy:
                return new IfEnemyCommand();
            case IfType.Event:
                return new IfEventCommand();
            case IfType.Gold:
                return new IfGoldCommand();
            case IfType.Item:
                return new IfItemCommand();
            case IfType.Weapon:
                return new IfWeaponCommand();
            case IfType.Armor:
                return new IfArmorCommand();
            case IfType.Button:
                return new IfButtonCommand();
            case IfType.Script:
                return new IfScriptCommand();
            case IfType.Vehicle:
                return new IfVehicleCommand();
            default:
                throw new Exception($"Unknown if type '{type}' command");
        }
    }
}
