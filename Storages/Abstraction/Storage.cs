using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Enums.Internal;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using System.Reflection;
using RPGMakerLibCoreV2.Diff;
using RPGMakerLibCoreV2.Storages.Serialization;

namespace RPGMakerLibCoreV2.Storages.Abstraction;

public abstract class Storage
{
    public string ProjectPath { get; private set; }

    public string DataDirectory { get; private set; }
    
    public StorageVersion Version { get; init; }

    public DataLookupHolder<IActor> Actors;
    public DataLookupHolder<IAnimation> Animations;
    public DataLookupHolder<IArmor> Armors;
    public DataLookupHolder<IClass> Classes;
    public DataLookupHolder<ICommonEvent> CommonEvents;
    public DataLookupHolder<IEnemy> Enemies;
    public DataLookupHolder<IItem> Items;
    public MapsHolder Maps;
    public DataLookupHolder<IMapInfo> MapInfos;
    public DataLookupHolder<ISkill> Skills;
    public DataLookupHolder<IState> States;
    public DataHolder<ISystem> System;
    public DataLookupHolder<ITileSet> Tilesets;
    public DataLookupHolder<ITroop> Troops;
    public DataLookupHolder<IWeapon> Weapons;

    private readonly List<ITable> _tables = new();
    private readonly IDataSerializer _serializer;
    private readonly TypeMap _types;

    protected Storage(StorageVersion version, IDataSerializer serializer, TypeMap types, string projectPath,
        string dataDirectory = "data")
    {
        Version = version;
        _serializer = serializer;
        _types = types;
        ProjectPath = projectPath;
        DataDirectory = dataDirectory;

        Init();
    }

    public void ChangeProjectPath(string path, string dataDirectory = "data")
    {
        ProjectPath = path;
        DataDirectory = dataDirectory;
    }

    public virtual void Unload()
    {
        foreach (var table in _tables)
            table.Unload();

        Maps.Unload();
    }

    protected abstract void Init();

    protected virtual string MapPath(int index) => $"Map{index:D3}{_serializer.FileExtension}";

    public virtual string DataPath(string dataObject) => Path.Combine(DataDirectory, dataObject);

    public bool DataFileExists(string dataFile) => File.Exists(Path.Combine(ProjectPath, DataDirectory, dataFile));

    public virtual T LoadData<T>(string dataFile)
    {
        var path = Path.Combine(ProjectPath, DataDirectory, dataFile);
        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        return _serializer.Load<T>(path);
    }

    public virtual void SaveData<T>(string dataFile, T data)
    {
        _serializer.Save(Path.Combine(ProjectPath, DataDirectory, dataFile), data);
    }

    protected string FileName(string name) => name + _serializer.FileExtension;

    public virtual void Preload()
    {
        foreach (var table in _tables)
            table.Preload();

        foreach (var map in MapInfos.Get())
            Maps.Get(map.Key);
    }

    public virtual void SaveAll()
    {
        foreach (var table in _tables)
            table.Save();

        Maps.Save();
    }

    private void SaveObject(string path, object? data) => SaveData(path, data);

    private THolder Register<THolder>(THolder holder) where THolder : ITable
    {
        _tables.Add(holder);
        return holder;
    }

    /// <summary>Concrete type of this storage's maker version for an entity interface.</summary>
    public Type ConcreteType<TV>() => _types.Resolve(typeof(TV));

    private object InvokeCore(string method, Type concrete, Type entity, params object?[] args)
    {
        return typeof(Storage)
            .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(concrete, entity)
            .Invoke(this, args)!;
    }

    protected void LoadListHolder<TV>(out DataLookupHolder<TV> holder, string fileName,
        Func<TV?, int>? idGetter = null) where TV : class
    {
        holder = (DataLookupHolder<TV>)InvokeCore(nameof(LoadListHolderCore), ConcreteType<TV>(), typeof(TV),
            fileName, idGetter);
    }

    protected void LoadDictionaryHolder<TV>(out DataLookupHolder<TV> holder, string fileName) where TV : class
    {
        holder = (DataLookupHolder<TV>)InvokeCore(nameof(LoadDictionaryHolderCore), ConcreteType<TV>(), typeof(TV),
            fileName);
    }

    protected void LoadHolder<TV>(out DataHolder<TV> holder, string fileName) where TV : class
    {
        holder = (DataHolder<TV>)InvokeCore(nameof(LoadHolderCore), ConcreteType<TV>(), typeof(TV), fileName);
    }

    protected void LoadMapsHolder(out MapsHolder holder)
    {
        holder = (MapsHolder)InvokeCore(nameof(LoadMapsHolderCore), ConcreteType<IMap>(), typeof(IMap));
    }

    private DataLookupHolder<TV> LoadListHolderCore<T, TV>(string fileName, Func<TV?, int>? idGetter)
        where T : class, TV
        where TV : class
    {
        return Register<DataLookupHolder<TV>>(new(HolderType.List, fileName, path =>
        {
            var data = LoadData<List<T>>(path);
            var dict = new Dictionary<int, TV?>();
            for (var i = 0; i < data.Count; ++i)
                dict[idGetter?.Invoke(data[i]) ?? i] = data[i];

            return dict;
        }, SaveObject));
    }

    private DataLookupHolder<TV> LoadDictionaryHolderCore<T, TV>(string fileName)
        where T : class, TV
        where TV : class
    {
        return Register<DataLookupHolder<TV>>(new(HolderType.Dictionary, fileName,
            path => LoadData<Dictionary<int, T?>>(path).MorphToBase<T, TV>(), SaveObject));
    }

    private DataHolder<TV> LoadHolderCore<T, TV>(string fileName)
        where T : class, TV
        where TV : class
    {
        return Register<DataHolder<TV>>(new(fileName, path => LoadData<T>(path), SaveObject));
    }

    private MapsHolder LoadMapsHolderCore<T, TV>()
        where T : class, TV
    {
        return new(MapPath, path => DataFileExists(path) ? (IMap)LoadData<T>(path)! : null, SaveObject);
    }

    public ICommonEvent? GetCommonEvent(int id)
    {
        return CommonEvents.Get(id);
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
        return MapInfos.Get(id);
    }

    public IAnimation? GetAnimation(int id)
    {
        return Animations.Get(id);
    }

    public IEvent? GetMapEvent(int mapId, int eventId)
    {
        return Maps.Get(mapId)?.Events.Get(eventId);
    }

    public IEventPage? GetMapEventPage(int mapId, int eventId, int pageId)
    {
        return GetMapEvent(mapId, eventId)?.Pages.ElementAt(pageId);
    }

    public class SearchInfo
    {
        public List<CommonEventContext> CommonEventResults = new();
        public List<MapEventContext> MapEventResults = new();
    }

    public delegate Task CommandWalker(int index, NamedCommand command, EntityContext context);
    
    // public delegate void CommandWalker<T>(int index, NamedCommand command, EntityContext context) where T: NamedCommand;

    public async Task SearchCommands(CommandWalker searcher, ProgressReporter.ProgressReporterCallback? reporterCallback = null,
        CancellationToken cancellationToken = default)
    {
        var reporter = reporterCallback != null ? new ProgressReporter(cancellationToken) : null;
        reporter?.Report += reporterCallback;

        reporter?.SetStage(DumpStage.Init);
        
        var stepCount = 0;
        stepCount += MapInfos.Get().Count;
        stepCount += Troops.Get().Sum(t => t.Value?.Pages.Count ?? 0);
        stepCount += CommonEvents.Get().Count;
        
        reporter?.ResetTotal(stepCount);
        
        reporter?.SetStage(DumpStage.Progress);

        async Task<bool> WalkCommands(IEnumerable<NamedCommand> commands, Func<EntityContext> contextFactory)
        {
            var commandIndex = 0;
            foreach (var command in commands)
            {
                try
                {
                    await searcher(commandIndex, command, contextFactory());
                }
                catch
                {
                    reporter?.SetStage(DumpStage.Finished);
                    throw;
                }

                ++commandIndex;

                if (reporter?.NeedCancel ?? false)
                    return false;
            }

            return true;
        }

        await CommonEventHelper.WalkCommonEvents(this, async (context) =>
        {
            using var _ = reporter?.DoReport("", 0);

            return await WalkCommands(context.CommonEvent.NamedCommands, () => new(this, context));
        }, reporter);

        var lastMapId = -1;
        await MapEventHelper.WalkMapEventPages(this, async (context) =>
        {
            if (lastMapId != context.MapId)
            {
                lastMapId = context.MapId;
                reporter?.DoReport("", 0).Finish();
            }

            return await WalkCommands(context.EventPage.NamedCommands, () => new(this, context));
        }, reporter);

        await TroopHelper.WalkTroopPages(this, async (context) =>
        {
            using var _ = reporter?.DoReport("", 0);

            return await WalkCommands(context.EventPage.NamedCommands, () => new(this, context));
        }, reporter);

        reporter?.SetStage(reporter.NeedCancel ? DumpStage.Cancelled : DumpStage.Finished);
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
