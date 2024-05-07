using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class TroopHelper
{
    public delegate void TroopWalker(TroopContext context);

    public static void WalkTroopPages(Storage storage, TroopWalker walker)
    {
        foreach (var troop in storage.Troops.Get())
        {
            if (troop == null)
                continue;

            foreach (var (i, p) in troop.Pages.WithIndex())
            {
                if (p == null)
                    continue;

                walker(new()
                {
                    Troop = troop,
                    PageId = i,
                    Page = p,
                });
            }
        }
    }

    public static void DumpTroopCommands(Storage storage, string path, DumpOptions? options = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        WalkTroopPages(storage, (context) =>
        {
            IndentHelper.ValidateIndents(context.Page, storage.Version);

            var fileName = $"Troop_{context.Troop.Id:D3}_Page_{context.PageId}.txt";
            File.WriteAllText(Path.Combine(path, fileName),
                CommandContainerHelper.Dump(context.Page, new()
                {
                    Storage = storage,
                    TroopContext = context,
                }, options));
        });
    }
}
