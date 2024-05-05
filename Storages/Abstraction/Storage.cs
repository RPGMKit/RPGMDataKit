using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Diff;

namespace RPGMakerLibCoreV2.Storages.Abstraction;

public abstract class Storage
{
    public string ProjectPath { get; private set; }
    
    public StorageVersion Version { get; init; }

    public DataHolder<IEnumerable<IAnimation>> Animations;
    public DataHolder<IEnumerable<ICommonEvent>> CommonEvents;
    public DataHolder<ISystem> System;
    public MapsHolder Maps;
    public DataHolder<IEnumerable<IMapInfo>> MapInfos;
    public DataHolder<IEnumerable<ITileSet>> Tilesets;

    public Storage(string projectPath)
    {
        ProjectPath = projectPath;

        Init();
    }

    protected abstract void Init();

    public abstract string DataPath(string dataObject);

    public abstract T LoadData<T>(string relativePath);

    public abstract void SaveData<T>(string relativePath, T data);

    public abstract void SaveAll();

    public ICommonEvent? GetCommonEvent(int id)
    {
        return CommonEvents.Get().ElementAtOrDefault(id);
    }

    public string GetSwitch(int id)
    {
        return System.Get().Switches.Get(id) ?? "";
    }

    public string GetVariable(int id)
    {
        return System.Get().Variables.Get(id) ?? "";
    }

    public IMapInfo? GetMapInfo(int id)
    {
        return MapInfos.Get().ElementAtOrDefault(id);
    }

    public IAnimation? GetAnimation(int id)
    {
        return Animations.Get().ElementAtOrDefault(id);
    }

    public void DumpCommonEvents(string path, DumpOptions options)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");

        CommonEventHelper.WalkCommonEvents(this, (commonEventId, container) =>
        {
            IndentHelper.ValidateIndents(container);

            File.WriteAllText(Path.Combine(path, $"CommonEvent_{commonEventId:D4}.txt"),
                GetCommonEvent(commonEventId).Dump(this, options));
        });
    }

    public void DumpMapEvents(string path, DumpOptions options)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist.");


        MapEventHelper.WalkMapEvents(this, (mapId, eventId, pageId, commands) =>
        {
            IndentHelper.ValidateIndents(commands);

            var fileName = $"Map_{mapId:D3}_Event_{eventId:D3}_Page_{pageId}.txt";
            File.WriteAllText(Path.Combine(path, fileName),
                GetMapEventPage(mapId, eventId, pageId).Dump(this, options));
        });
    }

    public IEvent GetMapEvent(int mapId, int eventId)
    {
        return Maps.Get(mapId)?.Events.ElementAt(eventId);
    }

    public IPage GetMapEventPage(int mapId, int eventId, int pageId)
    {
        return Maps.Get(mapId)?.Events.ElementAt(eventId)?.Pages.ElementAt(pageId);
    }

    public class CommonEventContextData
    {
        public Storage Storage;
        public ICommonEvent CommonEvent;

        public override string ToString() => $"Common Event #{CommonEvent.Id}";
    }

    public class MapEventContextData
    {
        public Storage Storage;
        public int MapId;
        public IMap Map;
        public IEvent Event;
        public int PageId;
        public IPage Page;

        public override string ToString() =>
            $"Map {MapId}, Event {Event.Id}, Page {PageId}, X {Event.X}, Y {Event.X}";
    }

    public class SearchInfo
    {
        public List<Tuple<CommonEventContextData, ICommandContainer>> CommonEventResults = new();
        public List<Tuple<MapEventContextData, ICommandContainer>> MapEventResults = new();
    }

    public delegate void CommandWalker(NamedCommand command, CommonEventContextData commonEventContext,
        MapEventContextData mapEventContext);
    
    public void SearchCommands(CommandWalker searcher)
    {
        CommonEventHelper.WalkCommonEvents(CommonEvents.Get(), (id, container) =>
        {
            int commandIndex = 0;
            foreach (var command in container.NamedCommands)
            {
                CommonEventContextData context = new() { CommonEvent = GetCommonEvent(id), Storage = this };

                searcher(command, context, null);
                ++commandIndex;
            }
        });

        MapEventHelper.WalkMapEvents(this, (mapId, eventId, pageId, container) =>
        {
            foreach (var command in container.NamedCommands)
            {
                var map = Maps.Get(mapId);
                var mapEvent = map.Events.ElementAt(eventId);
                var page = mapEvent.Pages.ElementAt(pageId);

                MapEventContextData context = new()
                {
                    Storage = this,
                    MapId = mapId,
                    Map = map,
                    Event = mapEvent,
                    PageId = pageId,
                    Page = page
                };

                searcher(command, null, context);
            }
        });
    }

    public class PatchOptions
    {
        public bool SkipRemoval = true;
        public bool IgnorePartial = false;
        public bool IgnoreNotExistsOnDelete = false;
        public bool IgnoreNotExistsOnModify = false;
        public bool IgnoreExistsOnNew = false;
    }

    public void ApplyPatches(List<Patch> patches, PatchOptions options)
    {
        // List<Tuple<Patch, string>> prepared = new();
        // foreach (var p in patches)
        // {
        //     string src = null;
        //     if (p.Variable != null)
        //     {
        //         src = GetVariable(p.Variable.VariableId);
        //     }
        //     else if (p.Switch != null)
        //     {
        //         src = GetSwitch(p.Switch.SwitchId);
        //     }
        //     else if (p.CommonEvent != null)
        //     {
        //         src = GetCommonEvent(p.CommonEvent.CommonEventId)
        //             ?.Dump(this, new DumpOptions() { Format = DumpFormat.PrettyJson });
        //         if (src == null && p.Type != DiffType.Added)
        //             throw new Exception($"Not found Common Event #{p.CommonEvent.CommonEventId}");
        //     }
        //     else if (p.Map != null)
        //     {
        //         src = Maps.Get(p.Map.MapId)?.Dump(this, new DumpOptions() { Format = DumpFormat.PrettyJson });
        //         if (src == null && p.Type != DiffType.Added)
        //             throw new Exception($"Not found Map #{p.Map.MapId}");
        //     }
        //     else if (p.MapEventPage != null)
        //     {
        //         src = GetMapEventPage(p.MapEventPage.MapId, p.MapEventPage.EventId, p.MapEventPage.PageId)
        //             .Dump(this, new DumpOptions() { Format = DumpFormat.PrettyJson });
        //         if (src == null && p.Type != DiffType.Added)
        //             throw new Exception(
        //                 $"Not found Map #{p.MapEventPage.MapId}, Event #{p.MapEventPage.EventId}, Page #{p.MapEventPage.PageId}");
        //     }
        //
        //     src ??= "";
        //
        //     if (p.Type == DiffType.Added)
        //     {
        //         if (!string.IsNullOrWhiteSpace(src) && !options.IgnoreExistsOnNew)
        //             throw new Exception("Patch is 'new' but entity already exist");
        //     }
        //     else if (p.Type == DiffType.Removed)
        //     {
        //         if (string.IsNullOrWhiteSpace(src) && !options.IgnoreNotExistsOnDelete)
        //             throw new Exception("Patch is 'deleted' but entity does not exist");
        //     }
        //     else if (p.Type == DiffType.Modified)
        //     {
        //         if (string.IsNullOrWhiteSpace(src))
        //         {
        //             if (!options.IgnoreNotExistsOnModify)
        //                 throw new Exception("Patch is 'modified' but entity does not exist");
        //         }
        //     }
        //
        //     var res = Differ.Patch(src, p.Patches);
        //     if (res.Status == Differ.PatchStatus.Error ||
        //         res.Status == Differ.PatchStatus.Partial && !options.IgnorePartial)
        //         throw new Exception("Failed to apply patch");
        //
        //     prepared.Add(new(p, res.Text));
        // }
        //
        // foreach (var (p, text) in prepared)
        // {
        //     if (p.Type == DiffType.Removed && options.SkipRemoval)
        //         continue;
        //
        //     if (p.Variable != null)
        //     {
        //         System.Get().Variables.Set(p.Variable.VariableId, text);
        //     }
        //     else if (p.Switch != null)
        //     {
        //         System.Get().Switches.Set(p.Switch.SwitchId, text);
        //     }
        //     else if (p.Map != null)
        //     {
        //         var obj = !string.IsNullOrWhiteSpace(text)
        //             ? JsonConvert.DeserializeObject<Map>(text)
        //             : null;
        //
        //         Maps.Set(p.Map.MapId, obj);
        //     }
        //     else if (p.MapInfo != null)
        //     {
        //         var obj = !string.IsNullOrWhiteSpace(text)
        //             ? JsonConvert.DeserializeObject<MapInfo>(text)
        //             : null;
        //
        //         MapInfos.Get().Set(p.MapInfo.MapId, obj);
        //     }
        //     else if (p.MapEvent != null)
        //     {
        //         var obj = !string.IsNullOrWhiteSpace(text)
        //             ? JsonConvert.DeserializeObject<Event>(text)
        //             : null;
        //
        //         Maps.Get(p.MapEvent.MapId)?.Events.Set(p.MapEvent.EventId, obj);
        //     }
        //     else if (p.CommonEvent != null)
        //     {
        //         var obj = !string.IsNullOrWhiteSpace(text)
        //             ? JsonConvert.DeserializeObject<CommonEvent>(text)
        //             : null;
        //         CommonEvents.Get().Set(p.CommonEvent.CommonEventId, obj);
        //     }
        //     else if (p.MapEventPage != null)
        //     {
        //         var obj = !string.IsNullOrWhiteSpace(text) ? JsonConvert.DeserializeObject<Event.Page>(text) : null;
        //
        //         var e = GetMapEvent(p.MapEventPage.MapId, p.MapEventPage.EventId);
        //         e?.Pages.Set(p.MapEventPage.PageId, obj);
        //     }
        // }
    }
}
