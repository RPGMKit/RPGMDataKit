using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages.Abstraction;

namespace RPGMDataKit.Data.Helpers;

public static class CommonEventHelper
{
    /// <summary>Called for each common event; set <see cref="CommonEventContext.StopWalking"/> to stop the walk after it.</summary>
    public delegate Task CommonEventWalker(CommonEventContext context);

    public static async Task WalkCommonEvents(Storage storage, CommonEventWalker walker, ProgressReporter? reporter = null)
    {
        using var _ = reporter?.IncLevel(storage.CommonEvents.Get().Count);

        foreach (var (_, c) in storage.CommonEvents.Get())
        {
            if (c == null)
                continue;

            using var _2 = reporter?.DoReport();
            var context = new CommonEventContext { CommonEvent = c };
            await walker(context);
            if (context.StopWalking)
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

            using var _2 = reporter?.DoReport("Common Event " + entityContext.NameFormatter.FormatCommonEvent(context.CommonEvent.Id));
            IndentHelper.ValidateIndents(context.CommonEvent, storage.Version);

            var filePath = Path.Combine(path, $"CommonEvent_{context.CommonEvent.Id:D3}.txt");
            await using var file = File.Open(filePath, FileMode.Create);

            await using var writer = new StreamWriter(file);
            await context.CommonEvent.Dump(writer, entityContext, options);
            
            reporter?.CustomReport(filePath);

            if (reporter?.NeedCancel ?? false)
                context.StopWalking = true;
        });
    }
}
