using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class CommonEventHelper
{
    public delegate void CommonEventWalker(CommonEventContext context);

    public static void WalkCommonEvents(Storage storage, CommonEventWalker walker)
    {
        WalkCommonEvents(storage.CommonEvents.Get(), walker);
    }

    public static void WalkCommonEvents(IEnumerable<ICommonEvent> commonEvents, CommonEventWalker walker)
    {
        foreach (var c in commonEvents)
        {
            if (c == null)
                continue;

            walker(new() { CommonEvent = c });
        }
    }

    public static void DumpCommonEventCommands(Storage storage, string path, DumpOptions? options = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        WalkCommonEvents(storage, (context) =>
        {
            IndentHelper.ValidateIndents(context.CommonEvent, storage.Version);

            File.WriteAllText(Path.Combine(path, $"CommonEvent_{context.CommonEvent.Id:D3}.txt"),
                context.CommonEvent.Dump(new()
                {
                    Storage = storage,
                    CommonEventContext = context,
                }, options));
        });
    }
}
