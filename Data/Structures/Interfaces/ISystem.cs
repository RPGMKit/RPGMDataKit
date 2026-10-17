namespace RPGMDataKit.Data.Structures.Interfaces;

public interface ISystem : IHasOptionalProperties
{
    public List<string> Switches { get; }

    public List<string> Variables { get; }

    public List<string> EquipmentTypes { get; }

    public string GameTitle { get; }

    public string CurrencyUnit { get; }

    public ITone WindowTone { get; set;  }
    
    public IList<ISE?> Sounds { get; }
}
