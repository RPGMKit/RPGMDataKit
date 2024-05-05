using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class CommonEventHelper
{
    public delegate void CommonEventWalker(int commonEventId, ICommandContainer container);

    public static void WalkCommonEvents(Storage storage, CommonEventWalker walker)
    {
        WalkCommonEvents(storage.CommonEvents.Get(), walker);
    }
    
    public static void WalkCommonEvents<T>(IEnumerable<ICommonEvent<T>> commonEvents, CommonEventWalker walker) where T: IEventCommand
    {
        foreach (var (i, c) in commonEvents.WithIndex())
        {
            if (c == null)
                continue;

            walker(i, c.List);
        }
    }
}
