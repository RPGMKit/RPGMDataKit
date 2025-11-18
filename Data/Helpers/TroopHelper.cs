using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class TroopHelper
{
    public delegate void TroopWalker(TroopContext context);

    public static void WalkTroopPages(Storage storage, TroopWalker walker)
    {
        foreach (var (_,troop) in storage.Troops.Get())
        {
            if (troop == null)
                continue;

            var i = 0;
            foreach (var p in troop.Pages)
            {
                if (p == null)
                    continue;

                walker(new()
                {
                    Troop = troop,
                    PageId = i,
                    EventPage = p,
                });

                ++i;
            }
        }
    }

    public static void DumpTroopCommands(Storage storage, string path, DumpOptions options)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        WalkTroopPages(storage, (context) =>
        {
            IndentHelper.ValidateIndents(context.EventPage, storage.Version);

            using var file = File.Open(Path.Combine(path, $"Troop_{context.Troop.Id:D3}_Page_{context.PageId}.txt"),
                FileMode.Create);
            using var writer = new StreamWriter(file);

            context.EventPage.Dump(writer, new(storage, context, options.ExtendedFormat), options);
        });
    }
}
