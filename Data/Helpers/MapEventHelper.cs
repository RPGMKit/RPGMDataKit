using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class MapEventHelper
{
    public delegate void MapEventWalker(int mapId, int eventId, int pageId, ICommandContainer container);

    public static void WalkMapEvents(Storage storage, MapEventWalker walker)
    {
        foreach (var info in storage.MapInfos.Get())
        {
            var m = storage.Maps.Get(info.Id);
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

                    walker(info.Id, e.Id, j, p);
                }
            }
        }
    }
}
