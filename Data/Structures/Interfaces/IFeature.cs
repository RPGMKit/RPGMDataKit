namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IFeature
{
    public int Code { get; set; }
    
    public int DataId { get; set; }

    public object Value { get; set; }
}
