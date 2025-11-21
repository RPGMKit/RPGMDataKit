using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class CommonEventHelper
{
    public delegate Task CommonEventWalker(CommonEventContext context);

    public static async Task WalkCommonEvents(Storage storage, CommonEventWalker walker)
    {
        foreach (var (_, c) in storage.CommonEvents.Get())
        {
            if (c == null)
                continue;

            await walker(new() { CommonEvent = c });
        }
    }

    public static async Task DumpCommonEventCommands(Storage storage, string path, DumpOptions options)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        await WalkCommonEvents(storage, async (context) =>
        {
            IndentHelper.ValidateIndents(context.CommonEvent, storage.Version);

            await using var file = File.Open(Path.Combine(path, $"CommonEvent_{context.CommonEvent.Id:D3}.txt"),
                FileMode.Create);

            await using var writer = new StreamWriter(file);
            await context.CommonEvent.Dump(writer, new(storage, context, options.ExtendedFormat), options);
        });
    }
}
