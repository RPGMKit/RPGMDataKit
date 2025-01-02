using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ITroop
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IByIdGetter<IMember> Members { get; }
    
    public IByIdGetter<ITroopPage> Pages { get; }
}