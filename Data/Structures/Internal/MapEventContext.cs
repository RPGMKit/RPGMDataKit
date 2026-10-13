using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Structures.Internal;

public class MapEventContext
{
    public int MapId { get; set; }
    public IMap Map { get; set; }
    public IEvent Event { get; set; }
    public int PageId { get; set; }
    public IEventPage EventPage { get; set; }
    
    public override string ToString() =>
        $"Map [{MapId:D3}:{Map.DisplayName}], Event [{Event.Id:D3}:{Event.Name}], Page {PageId}, X {Event.X}, Y {Event.Y}";
}
