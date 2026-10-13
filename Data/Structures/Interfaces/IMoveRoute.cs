namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IMoveRoute : ICommandContainer
{
    public bool Repeat { get; set; }

    public bool Skippable { get; set; }

    public bool Wait { get; set; }
}