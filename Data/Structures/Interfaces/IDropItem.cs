namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IDropItem
{
    public int Kind { get; set; }

    public int DataId { get; set; }

    public int Denominator { get; set; }
}