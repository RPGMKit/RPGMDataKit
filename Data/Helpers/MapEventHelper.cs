using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages.Abstraction;

namespace RPGMDataKit.Data.Helpers;

public static class MapEventHelper
{
    public delegate Task<bool> MapEventPageWalker(MapEventContext context);

    public static async Task WalkMapEventPages(Storage storage, MapEventPageWalker pageWalker,
        ProgressReporter? reporter = null)
    {
        using var _ = reporter?.IncLevel(storage.MapInfos.Get().Count);

        foreach (var (id, info) in storage.MapInfos.Get())
        {
            var m = storage.Maps.Get(id);
            if (m == null)
                continue;

            using var _2 = reporter?.DoReport();
            foreach (var (_, e) in m.Events)
            {
                if (e == null)
                    continue;

                var i = 0;
                foreach (var p in e.Pages)
                {
                    if (p == null)
                        continue;

                    if (!await pageWalker(new()
                        {
                            MapId = id,
                            Map = m,
                            Event = e,
                            PageId = i,
                            EventPage = p,
                        }))
                        return;
                    ++i;
                }
            }
        }
    }

    public static async Task DumpMapEventCommands(Storage storage, string path, DumpOptions options, ProgressReporter? reporter = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        using var _ = reporter?.IncLevel(storage.MapInfos.Get().Count);

        var lastMapId = -1;
        await WalkMapEventPages(storage, async (context) =>
        {
            var entityContext = new EntityContext(storage, context, options.ExtendedFormat);

            if (lastMapId != context.MapId)
            {
                lastMapId = context.MapId;
                reporter?.DoReport("Map " + entityContext.NameFormatter.FormatMap(context.MapId)).Finish();
            }

            IndentHelper.ValidateIndents(context.EventPage, storage.Version);

            var filePath = Path.Combine(path, $"Map_{context.MapId:D3}_Event_{context.Event.Id:D3}_Page_{context.PageId}.txt");
            await using var file = File.Open(filePath, FileMode.Create);
            await using var writer = new StreamWriter(file);

            await context.EventPage.Dump(writer, entityContext, options);
            
            reporter?.CustomReport(filePath);

            return !(reporter?.NeedCancel ?? false);
        });
    }

    public static async Task DumpPageInfo(IEventPage page, TextWriter writer, EntityContext context, DumpOptions options)
    {
        if (context.MapEventContext != null)
        {
            await writer.WriteLineAsync($"Map: {context.NameFormatter.FormatMap(context.MapEventContext!.MapId)}");
            await writer.WriteLineAsync(
                $"Event: {context.NameFormatter.FormatEvent(context.MapEventContext.Event.Id)}, Page: {context.MapEventContext.PageId}");
            await writer.WriteLineAsync(
                $"Coords: ({context.MapEventContext.Event.X}, {context.MapEventContext.Event.Y})");
        }

        await writer.WriteLineAsync("Character: " + (!string.IsNullOrWhiteSpace(page.Graphic.CharacterName) ? $"{page.Graphic.CharacterName}, {page.Graphic.CharacterIndex}" : ""));
        await writer.WriteLineAsync($"Tile Id: {page.Graphic.TileId}");
        await writer.WriteLineAsync($"Trigger: {page.Trigger}");
        await writer.WriteLineAsync("Condition:");
        await DumpConditionInfo(page.Condition, writer, context, options);
    }

    private static async Task DumpConditionInfo(IEventPageCondition condition, TextWriter writer, EntityContext context,
        DumpOptions options)
    {
        await writer.WriteLineAsync("Switch 1: " + (condition.Switch1Valid
            ? $"{context.NameFormatter.FormatSwitch(condition.Switch1Id)} is ON"
            : "-"));
        await writer.WriteLineAsync("Switch 2: " + (condition.Switch2Valid
            ? $"{context.NameFormatter.FormatSwitch(condition.Switch2Id)} is ON"
            : "-"));
        await writer.WriteLineAsync(
            "Variable: " + (condition.VariableValid
                ? $"{context.NameFormatter.FormatVariable(condition.VariableId, new(){BracketType = BracketType.Square})} == {condition.VariableValue}"
                : "-"));
        await writer.WriteLineAsync("Self Switch: " + (condition.SelfSwitchValid ? condition.SelfSwitchCh : "-"));
        await writer.WriteLineAsync("Item: " +
                                    (condition.ItemValid ? context.NameFormatter.FormatItem(condition.ItemId) : "-"));
        await writer.WriteLineAsync("Actor: " +
                                    (condition.ActorValid ? context.NameFormatter.FormatActor(condition.ActorId) : "-"));
    }
}
