using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
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
        
    public SourceType SourceT { get; set; }
    public int SourceId { get; set; }
    public OperatorType Operator { get; set; }
    public int SkillId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        SourceT = parameterProvider.GetParameter<SourceType>(0);
        SourceId = parameterProvider.GetParameter<int>(1);
        Operator = parameterProvider.GetParameter<OperatorType>(2);
        SkillId = parameterProvider.GetParameter<int>(3);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, SourceT);
        parameterProvider.SetParameter(1, SourceId);
        parameterProvider.SetParameter(2, Operator);
        parameterProvider.SetParameter(3, SkillId);
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