namespace RPGMDataKit.Data.Structures.Interfaces;

public interface ISkill : IHasId
{
    public string Name { get; set; }
    int IconIndex { get; set; }
}
