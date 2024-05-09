using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ISystem
{
    public LookupList<string> Switches { get; }

    public LookupList<string> Variables { get; }
    
    public LookupList<string> EquipmentTypes { get; }
}
