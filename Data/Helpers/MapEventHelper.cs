using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class MapEventHelper
{
    public delegate void MapEventWalker(int mapId, int eventId, int pageId, ICommandContainer container);

    public static void WalkMapEvents(Storage storage, MapEventWalker walker)
    {
        foreach (var (id,info) in storage.MapInfos.Get().WithIndex())
        {
            var m = storage.Maps.Get(id);
            if (m == null)
                continue;

            foreach (var e in m.Events)
            {
                if (e == null)
                    continue;

                foreach (var (j, p) in e.Pages.WithIndex())
                {
                    if (p == null)
                        continue;

                    walker(id, e.Id, j, p);
                }
            }
        }
    }

    public static void DumpMapEvents(Storage storage, string path, DumpOptions options)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        WalkMapEvents(storage, (mapId, eventId, pageId, commands) =>
        {
            IndentHelper.ValidateIndents(commands, storage.Version);

            var fileName = $"Map_{mapId:D3}_Event_{eventId:D3}_Page_{pageId}.txt";
            File.WriteAllText(Path.Combine(path, fileName),
                storage.GetMapEventPage(mapId, eventId, pageId).Dump(storage, options));
        });
    }
}
