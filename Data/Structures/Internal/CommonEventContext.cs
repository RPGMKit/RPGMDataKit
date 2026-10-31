using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Structures.Internal;

public class CommonEventContext
{
    public ICommonEvent CommonEvent { get; set; }

    /// <summary>Set to true by a walker (<see cref="Helpers.CommonEventHelper.WalkCommonEvents"/>) to stop the walk after this common event.</summary>
    public bool StopWalking { get; set; }

    public override string ToString() =>
        $"CommonEvent [{CommonEvent.Id:D3}:{CommonEvent.Name}]";
}