using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfEnemyCommand : IfCommand
{
    public enum CheckType
    {
        Appeared = 0,
        State = 1,
    }

    public int TroopId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public CheckType Check
    {
        get => Get<CheckType>(2);
        set => Set(2, value);
    }

    public int Param
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(TroopId);

        switch (Check)
        {
            case CheckType.Appeared:
                if (context.Version == StorageVersion.MV)
                    str += " is appeared";
                str += " is Appeared";
                break;
            case CheckType.State:
                if (context.Version == StorageVersion.MV)
                    str += $" is affected by {context.NameFormatter.FormatState(Param)}";
                else
                    str += $" is {context.NameFormatter.FormatState(Param)} Inflicted";
                break;
            default:
                throw new Exception(
                    $"Unknown check type '{Check}' for '{GetCommandName(context.Version)}' enemy command");
        }

        return str;
    }
}