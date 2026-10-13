namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IAnimation : IHasId
{

    public string Name { get; set; }

    public string Animation1Name { get; set; }

    public string Animation2Name { get; set; }
}
