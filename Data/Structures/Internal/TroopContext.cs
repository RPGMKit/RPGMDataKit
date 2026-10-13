using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Structures.Internal;

public class TroopContext
{
    public ITroop Troop { get; set; }
    public int PageId { get; set; }
    public ITroopPage EventPage { get; set; }
    
    public override string ToString() =>
        $"Troop [{Troop.Id:D3}:{Troop.Name}], Page {PageId}";
}