using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class TroopHelper
{
    public delegate Task TroopWalker(TroopContext context);

    public static async Task WalkTroopPages(Storage storage, TroopWalker walker)
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

                await walker(new()
                {
                    Troop = troop,
                    PageId = i,
                    EventPage = p,
                });

                ++i;
            }
        }
    }

    public static async Task DumpTroopCommands(Storage storage, string path, DumpOptions options)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        await WalkTroopPages(storage, async (context) =>
        {
            IndentHelper.ValidateIndents(context.EventPage, storage.Version);

            await using var file = File.Open(
                Path.Combine(path, $"Troop_{context.Troop.Id:D3}_Page_{context.PageId}.txt"),
                FileMode.Create);
            await using var writer = new StreamWriter(file);

            await context.EventPage.Dump(writer, new(storage, context, options.ExtendedFormat), options);
        });
    }
}
