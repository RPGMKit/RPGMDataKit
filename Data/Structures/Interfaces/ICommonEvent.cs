namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommonEvent : ICommandContainer
{
    public int Id { get; set; }

    public string Name { get; set; }
}