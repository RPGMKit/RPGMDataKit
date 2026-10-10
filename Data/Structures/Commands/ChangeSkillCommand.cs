using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeSkillCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeSkill;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Skill";
        else
            return "Change Skills";
    }
        
    public enum SourceType
    {
        Actor = 0,
        Variable = 1,
    }
        
    public enum OperatorType
    {
        Learn = 0,
        Forget = 1,
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

    public int SkillId
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
            _ => throw new Exception($"Unknown change skill source type '{SourceT}'")
        };

        str += Operator switch
        {
            OperatorType.Learn => ", + ",
            OperatorType.Forget => ", - ",
            _ => throw new Exception($"Unknown change skill operator '{Operator}'")
        };

        str += context.NameFormatter.FormatSkill(SkillId);

        return str;
    }
}