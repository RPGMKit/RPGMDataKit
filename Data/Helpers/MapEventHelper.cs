using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class MapEventHelper
{
    public delegate Task MapEventPageWalker(MapEventContext context);

    public static async Task WalkMapEventPages(Storage storage, MapEventPageWalker pageWalker)
    {
        foreach (var (id, info) in storage.MapInfos.Get())
        {
            var m = storage.Maps.Get(id);
            if (m == null)
                continue;

            foreach (var e in m.Events)
            {
                if (e == null)
                    continue;

                var i = 0;
                foreach (var p in e.Pages)
                {
                    if (p == null)
                        continue;

                    await pageWalker(new()
                    {
                        MapId = id,
                        Map = m,
                        Event = e,
                        PageId = i,
                        EventPage = p,
                    });
                    ++i;
                }
            }
        }
    }

    public static async Task DumpMapEventCommands(Storage storage, string path, DumpOptions options)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        await WalkMapEventPages(storage, async (context) =>
        {
            IndentHelper.ValidateIndents(context.EventPage, storage.Version);

            await using var file = File.Open(
                Path.Combine(path, $"Map_{context.MapId:D3}_Event_{context.Event.Id:D3}_Page_{context.PageId}.txt"),
                FileMode.Create);
            await using var writer = new StreamWriter(file);

            await context.EventPage.Dump(writer, new(storage, context, options.ExtendedFormat), options);
        });
    }
}
