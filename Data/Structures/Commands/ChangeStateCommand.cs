using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeStateCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeState;
    public override string GetCommandName(StorageVersion version) => "Change State";
        
    public enum SourceType
    {
        Actor = 0,
        Variable = 1,
    }

    public enum OperatorType
    {
        Add = 0,
        Remove = 1,
    }

    public SourceType SourceT
    {
        get => Get<SourceType>(0);
        set => Set(0, value);
    }

    public int SourceId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public OperatorType Operator
    {
        get => Get<OperatorType>(2);
        set => Set(2, value);
    }

    [EditorDefault(1, StorageVersion.VXAce)]
    public int StateId
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = SourceT switch
        {
            SourceType.Actor => context.NameFormatter.FormatActor(SourceId),
            SourceType.Variable => context.NameFormatter.FormatVariable(SourceId),
            _ => throw new Exception($"Unknown change state source type '{SourceT}'")
        };

        str += Operator switch
        {
            OperatorType.Add => ", + ",
            OperatorType.Remove => ", - ",
            _ => throw new Exception($"Unknown modify state operator '{Operator}'")
        };

        str += context.NameFormatter.FormatState(StateId);

        return str;
    }
}