using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class TroopHelper
{
    public delegate Task<bool> TroopWalker(TroopContext context);

    public static async Task WalkTroopPages(Storage storage, TroopWalker walker, ProgressReporter? reporter = null)
    {
        using var _ = reporter?.IncLevel(storage.Troops.Get().Count);
        
        foreach (var (_,troop) in storage.Troops.Get())
        {
            if (troop == null)
                continue;

            using var _2 = reporter?.Report();
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

    public static async Task DumpTroopCommands(Storage storage, string path, DumpOptions options, ProgressReporter? reporter = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        using var _ = reporter?.IncLevel(storage.Troops.Get().Sum(m => m.Value?.Pages.Count() ?? 0));
        
        await WalkTroopPages(storage, async (context) =>
        {
            var entityContext = new EntityContext(storage, context, options.ExtendedFormat);
            
            using var _2 = reporter?.Report("Troop " + entityContext.NameFormatter.FormatTroop(context.Troop.Id));
            
            IndentHelper.ValidateIndents(context.EventPage, storage.Version);

            await using var file = File.Open(
                Path.Combine(path, $"Troop_{context.Troop.Id:D3}_Page_{context.PageId}.txt"),
                FileMode.Create);
            await using var writer = new StreamWriter(file);

            await context.EventPage.Dump(writer, new(storage, context, options.ExtendedFormat), options);
            
            return !reporter?.NeedCancel ?? false;
        });
    }
    
    public static async Task DumpPageInfo(ITroopPage page, TextWriter writer, EntityContext context, DumpOptions options)
    {
        await writer.WriteLineAsync($"Troop: {context.NameFormatter.FormatTroop(context.TroopContext!.Troop.Id)}, Page: {context.TroopContext.PageId}");
        await DumpConditionInfo(page.Condition, writer, context, options);
    }

    private static async Task DumpConditionInfo(ITroopPageCondition condition, TextWriter writer, EntityContext context,
        DumpOptions options)
    {
        await writer.WriteLineAsync("Turn End: " + (condition.TurnEnding ? "True" : "False"));
        await writer.WriteLineAsync("Turn: " +
                                    (condition.TurnValid ? $"{condition.TurnA} + {condition.TurnB} * X" : "-"));
        await writer.WriteLineAsync("Enemy HP: " + (condition.EnemyValid
            ? $"{context.NameFormatter.FormatTroopMember(condition.EnemyIndex)} {condition.EnemyHp}% or below"
            : "-"));
        await writer.WriteLineAsync("Actor: " + (condition.ActorValid
            ? $"{context.NameFormatter.FormatActor(condition.ActorId)} {condition.ActorHp}% or below"
            : "-"));
        await writer.WriteLineAsync("Switch: " + (condition.SwitchValid
            ? $"{context.NameFormatter.FormatSwitch(condition.SwitchId)} is ON"
            : "-"));
    }
}
