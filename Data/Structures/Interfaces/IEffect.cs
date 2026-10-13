namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IEffect
{
    public int Code { get; set; }

    public int DataId { get; set; }

    public object Value1 { get; set; }

    public object Value2 { get; set; }
}
