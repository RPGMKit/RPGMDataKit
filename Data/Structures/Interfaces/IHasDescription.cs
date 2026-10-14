namespace RPGMDataKit.Data.Structures.Interfaces;

/// <summary>An entity with a description (the text of the help window): skills, items, weapons, armors in both makers.</summary>
public interface IHasDescription
{
    public string Description { get; set; }
}
