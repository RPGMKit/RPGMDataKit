namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IEvent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IList<IEventPage?> Pages { get; }
    public int X { get; set; }
    public int Y { get; set; }
}