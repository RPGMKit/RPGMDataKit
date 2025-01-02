namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IAction
{
    public int SkillId { get; set; }

    public int ConditionType { get; set; }

    public object ConditionParam1 { get; set; }

    public object ConditionParam2 { get; set; }

    public int Rating { get; set; }
}