using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class CommonEventHelper
{
    public delegate Task<bool> CommonEventWalker(CommonEventContext context);

    public static async Task WalkCommonEvents(Storage storage, CommonEventWalker walker, ProgressReporter? reporter = null)
    {
        using var _ = reporter?.IncLevel(storage.CommonEvents.Get().Count);

        foreach (var (_, c) in storage.CommonEvents.Get())
        {
            if (c == null)
                continue;

            using var _2 = reporter?.Report();
            if (!await walker(new() { CommonEvent = c }))
                break;
        }
    }

    public static async Task DumpCommonEventCommands(Storage storage, string path, DumpOptions options,
        ProgressReporter? reporter = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        using var _ = reporter?.IncLevel(storage.CommonEvents.Get().Count);
        
        await WalkCommonEvents(storage, async (context) =>
        {
            var entityContext = new EntityContext(storage, context, options.ExtendedFormat);

            using var _2 = reporter?.Report("Common Event " + entityContext.NameFormatter.FormatCommonEvent(context.CommonEvent.Id));
            IndentHelper.ValidateIndents(context.CommonEvent, storage.Version);

            await using var file = File.Open(Path.Combine(path, $"CommonEvent_{context.CommonEvent.Id:D3}.txt"),
                FileMode.Create);

            await using var writer = new StreamWriter(file);
            await context.CommonEvent.Dump(writer, entityContext, options);

            return !reporter?.NeedCancel ?? false;
        });
    }
}
