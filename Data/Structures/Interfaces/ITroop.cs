namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ITroop : IHasId
{

    public string Name { get; set; }

    public IList<IMember?> Members { get; }
    
    public IList<ITroopPage?> Pages { get; }
}