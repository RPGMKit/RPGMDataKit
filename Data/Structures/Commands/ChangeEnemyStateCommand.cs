using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeEnemyStateCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeEnemyState;
    public override string GetCommandName(StorageVersion version) => "Change Enemy State";
        
    public enum OperatorType
    {
        Add = 0,
        Remove = 1,
    }

    public int TroopId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public OperatorType Operator
    {
        get => Get<OperatorType>(1);
        set => Set(1, value);
    }

    public int StateId
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(TroopId);

        str += Operator switch
        {
            OperatorType.Add => ", + ",
            OperatorType.Remove => ", - ",
            _ => throw new Exception($"Unknown change enemy state operator '{Operator}'")
        };

        str += context.NameFormatter.FormatState(StateId);

        return str;
    }
}