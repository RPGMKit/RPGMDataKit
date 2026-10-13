namespace RPGMDataKit.Data.Structures.Internal.Converters;

/// <summary>A dictionary read from a JSON array: <see cref="Slots"/> is the length of that array, null slots included.</summary>
public class SlotDictionary<TValue> : Dictionary<int, TValue>
{
    public int Slots { get; set; }
}
