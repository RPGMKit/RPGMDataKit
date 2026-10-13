using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Structures.Internal;

public class CommonEventContext
{
    public ICommonEvent CommonEvent { get; set; }

    public override string ToString() =>
        $"CommonEvent [{CommonEvent.Id:D3}:{CommonEvent.Name}]";
}