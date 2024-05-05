using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IEvent : IDumpable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }
    public IEnumerable<IPage> Pages { get; }
    public int X { get; set; }
    public int Y { get; set; }
}