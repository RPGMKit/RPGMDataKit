namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IEventPageCondition
{
    public bool Switch1Valid { get; set; }

    public bool Switch2Valid { get; set; }

    public bool VariableValid { get; set; }

    public bool SelfSwitchValid { get; set; }

    public bool ItemValid { get; set; }

    public bool ActorValid { get; set; }

    public int Switch1Id { get; set; }

    public int Switch2Id { get; set; }

    public int VariableId { get; set; }

    public int VariableValue { get; set; }

    public string SelfSwitchCh { get; set; }

    public int ItemId { get; set; }

    public int ActorId { get; set; }
}