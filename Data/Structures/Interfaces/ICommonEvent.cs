namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommonEvent : ICommandContainer<ICommandWithIndent>
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    public int Trigger { get; set; }
    
    public int SwitchId { get; set; }
}