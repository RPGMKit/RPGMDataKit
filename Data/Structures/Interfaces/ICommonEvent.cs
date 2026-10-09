namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommonEvent : IHasId, ICommandContainer<ICommand>
{

    public string Name { get; set; }
    
    public int Trigger { get; set; }
    
    public int SwitchId { get; set; }
}