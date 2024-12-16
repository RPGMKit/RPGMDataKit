using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ForceActionCommand : NamedCommand
{
    public override int Code => CommandCodes.ForceAction;
    public override string GetCommandName(StorageVersion version) => "Force Action";
        
    public enum SubjectType
    {
        Enemy = 0,
        Actor = 1,
    }

    public enum TargetType
    {
        LastTarget = -2,
        Random = -1,
        Index1 = 0,
        Index2 = 1,
        Index3 = 2,
        Index4 = 3,
        Index5 = 4,
        Index6 = 5,
        Index7 = 6,
        Index8 = 7,
    }

    public SubjectType Subject { get; set; }
    public int SubjectId { get; set; }
    public int SkillId { get; set; }
    public TargetType Target { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Subject = parameterProvider.GetParameter<SubjectType>(0);
        SubjectId = parameterProvider.GetParameter<int>(1);
        SkillId = parameterProvider.GetParameter<int>(2);
        Target = parameterProvider.GetParameter<TargetType>(3);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Subject);
        parameterProvider.SetParameter(1, SubjectId);
        parameterProvider.SetParameter(2, SkillId);
        parameterProvider.SetParameter(3, Target);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Subject switch
        {
            SubjectType.Enemy => context.NameFormatter.FormatTroopMember(SubjectId),
            SubjectType.Actor => context.NameFormatter.FormatActor(SubjectId),
            _ => throw new Exception($"Unknown subject type {Subject}")
        };

        str += $", {context.NameFormatter.FormatSkill(SkillId)}";

        str += Target switch
        {
            TargetType.LastTarget => ", Last Target",
            TargetType.Random => ", Random",
            _ => $", Index {(int)Target + 1}"
        };

        return str;
    }
}