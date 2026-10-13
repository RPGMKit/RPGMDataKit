using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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

    public SubjectType Subject
    {
        get => Get<SubjectType>(0);
        set => Set(0, value);
    }

    public int SubjectId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public int SkillId
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public TargetType Target
    {
        get => Get<TargetType>(3);
        set => Set(3, value);
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