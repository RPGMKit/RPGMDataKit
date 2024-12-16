namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IMoveRoute : ICommandContainer<IMoveCommand>
{
    public bool Repeat { get; set; }

    public bool Skippable { get; set; }

    public bool Wait { get; set; }
}