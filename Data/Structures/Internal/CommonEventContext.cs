using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class CommonEventContext
{
    public ICommonEvent CommonEvent { get; set; }

    public override string ToString() =>
        $"CommonEvent [{CommonEvent.Id:D3}:{CommonEvent.Name}]";
}