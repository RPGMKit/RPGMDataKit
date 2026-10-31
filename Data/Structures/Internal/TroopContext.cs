using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Structures.Internal;

public class TroopContext
{
    public ITroop Troop { get; set; }
    public int PageId { get; set; }
    public ITroopPage EventPage { get; set; }

    /// <summary>Set to true by a walker (<see cref="Helpers.TroopHelper.WalkTroopPages"/>) to stop the walk after this page.</summary>
    public bool StopWalking { get; set; }

    public override string ToString() =>
        $"Troop [{Troop.Id:D3}:{Troop.Name}], Page {PageId}";
}