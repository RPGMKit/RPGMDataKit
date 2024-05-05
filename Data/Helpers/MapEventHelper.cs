using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class MapEventHelper
{
    public delegate void MapEventWalker(int mapId, int eventId, int pageId, ICommandContainer container);

    public static void WalkMapEvents(Storage storage, MapEventWalker walker)
    {
        for (var i = 0; i < storage.MapInfos.Get().Count; ++i)
        {
            var m = storage.Maps.Get(i);
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

                    walker(i, e.Id, j, p.List);
                }
            }
        }
    }
}
