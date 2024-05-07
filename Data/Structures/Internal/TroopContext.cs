using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class TroopContext
{
    public ITroop Troop { get; set; }
    public int PageId { get; set; }
    public IPage Page { get; set; }
    
    public override string ToString() =>
        $"Troop [{Troop.Id:D3}:{Troop.Name}], Page {PageId}";
}