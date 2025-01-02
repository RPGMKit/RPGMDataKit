using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IEvent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IByIdGetter<IEventPage> Pages { get; }
    public int X { get; set; }
    public int Y { get; set; }
}