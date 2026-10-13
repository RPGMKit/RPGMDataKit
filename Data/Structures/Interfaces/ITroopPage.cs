namespace RPGMDataKit.Data.Structures.Interfaces;

public interface ITroopPage : IPage
{
    public int Span { get; set; }
    
    public ITroopPageCondition Condition { get; }
}
