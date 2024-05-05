using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ISystem
{
    public LookupList<string> Switches { get; set; }

    public LookupList<string> Variables { get; set; }
}
