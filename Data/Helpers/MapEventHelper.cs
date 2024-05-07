using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class MapEventHelper
{
    public delegate void MapEventPageWalker(MapEventContext context);

    public static void WalkMapEventPages(Storage storage, MapEventPageWalker pageWalker)
    {
        foreach (var (id, info) in storage.MapInfos.Get().WithIndex())
        {
            var m = storage.Maps.Get(id);
            if (m == null)
                continue;

            foreach (var e in m.Events)
            {
                if (e == null)
                    continue;

                foreach (var (i, p) in e.Pages.WithIndex())
                {
                    if (p == null)
                        continue;

                    pageWalker(new()
                    {
                        MapId = id,
                        Map = m,
                        Event = e,
                        PageId = i,
                        Page = p,
                    });
                }
            }
        }
    }

    public static void DumpMapEventCommands(Storage storage, string path, DumpOptions? options = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        WalkMapEventPages(storage, (context) =>
        {
            IndentHelper.ValidateIndents(context.Page, storage.Version);

            var fileName = $"Map_{context.MapId:D3}_Event_{context.Event.Id:D3}_Page_{context.PageId}.txt";
            File.WriteAllText(Path.Combine(path, fileName),
                context.Page.Dump(new()
                {
                    Storage = storage,
                    MapEventContext = context,
                }, options));
        });
    }
}
