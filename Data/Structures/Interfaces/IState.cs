namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IState : IHasId
{
    public string Name { get; set; }
    public int IconIndex { get; set; }
}
