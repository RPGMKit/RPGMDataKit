using RPGMDataKit.Data;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Enums.Internal;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using System.Reflection;
using RPGMDataKit.Diff;
using RPGMDataKit.Storages.Serialization;

namespace RPGMDataKit.Storages.Abstraction;

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

    // the holders that Preload, SaveAll and Unload go through (the maps are handled separately)
    private readonly List<IHolder> _managed = new();
    private readonly IDataSerializer _serializer;
    private readonly TypeMap _types;
    private readonly HashSet<string> _files = new(StringComparer.OrdinalIgnoreCase);
    // every holder by its name, the name of its data file without the extension ("Actors", "Maps", "Quests")
    private readonly Dictionary<string, object> _holders = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Raised while a list or a dictionary is loaded, for each entity whose own <see cref="IHasId.Id"/> differs from its place
    /// (the index in the list, the key in the dictionary). The engines look an entity up by its place and take <c>entity.id</c> back from it,
    /// so the two must be equal; the place is what counts and the entity is left as it is. Tables are loaded when first read:
    /// subscribe before that, or call <see cref="Preload"/> after subscribing.
    /// </summary>
    public event Action<IdMismatch>? IdMismatched;

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
        foreach (var holder in _managed)
            holder.Unload();

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
        foreach (var holder in _managed)
            holder.Preload();

        foreach (var map in MapInfos.Get())
            Maps.Get(map.Key);
    }

    public virtual void SaveAll()
    {
        foreach (var holder in _managed)
            holder.Save();

        Maps.Save();
    }

    private void SaveObject(string path, object? data) => SaveData(path, data);

    private THolder Register<THolder>(string fileName, THolder holder) where THolder : class, IHolder
    {
        _managed.Add(holder);
        _holders[Path.GetFileNameWithoutExtension(fileName)] = holder;
        return holder;
    }

    /// <summary>Concrete type of this storage's maker version for an entity interface.</summary>
    public Type ConcreteType<TV>() => _types.Resolve(typeof(TV));

    /// <summary>
    /// A new entity of this storage's maker version with the values that the editor gives to a new one, not in any list yet:
    /// <c>storage.Actors.Add(storage.New&lt;IActor&gt;())</c>.
    /// </summary>
    public T New<T>() where T : class => _types.New<T>();

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

    /// <param name="idIsStored">
    /// Whether an entry keeps its own id. If so, it is compared with the key (<see cref="IdMismatched"/>); if not (VX Ace map infos),
    /// the key is put into the entity's id when the dictionary is loaded.
    /// </param>
    protected void LoadDictionaryHolder<TV>(out DataLookupHolder<TV> holder, string fileName, bool idIsStored = true)
        where TV : class
    {
        holder = (DataLookupHolder<TV>)InvokeCore(nameof(LoadDictionaryHolderCore), ConcreteType<TV>(), typeof(TV),
            fileName, idIsStored);
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
        _files.Add(fileName);

        return Register<DataLookupHolder<TV>>(fileName, new(HolderType.List, fileName, path =>
        {
            var data = LoadData<List<T>>(path);
            var dict = new Dictionary<int, TV?>();
            for (var i = 0; i < data.Count; ++i)
                dict[idGetter?.Invoke(data[i]) ?? i] = data[i];

            if (idGetter == null)
                ReportIdMismatches(path, dict);

            return dict;
        }, SaveObject));
    }

    private DataLookupHolder<TV> LoadDictionaryHolderCore<T, TV>(string fileName, bool idIsStored)
        where T : class, TV
        where TV : class
    {
        _files.Add(fileName);

        return Register<DataLookupHolder<TV>>(fileName, new(HolderType.Dictionary, fileName, path =>
        {
            var dict = LoadData<Dictionary<int, T?>>(path).MorphToBase<T, TV>();
            if (idIsStored)
            {
                ReportIdMismatches(path, dict);
            }
            else
            {
                foreach (var (key, item) in dict)
                {
                    if (item is IHasId entity)
                        entity.Id = key;
                }
            }

            return dict;
        }, SaveObject));
    }

    private DataHolder<TV> LoadHolderCore<T, TV>(string fileName)
        where T : class, TV
        where TV : class
    {
        _files.Add(fileName);

        return Register<DataHolder<TV>>(fileName, new(fileName, path => LoadData<T>(path), SaveObject));
    }

    private void ReportIdMismatches<TV>(string file, Dictionary<int, TV?> items) where TV : class
    {
        if (IdMismatched == null)
            return;

        foreach (var (key, item) in items)
        {
            if (item is IHasId entity && entity.Id != key)
                IdMismatched?.Invoke(new IdMismatch(file, key, entity.Id, item));
        }
    }

    /// <summary>
    /// Adds a list of entities kept in its own data file, <c>&lt;name&gt;</c> plus the extension of the version (a JSON array in MV).
    /// The id of an entity is its index in the list, as the engines take it; slot 0 is not used. The entity must have an id
    /// (<see cref="IHasId"/>) and keep the members it does not describe (for MV, derive from <c>JsonEntity</c>).
    /// </summary>
    /// <param name="optional">The file may be absent: the list starts empty and the file is written when it is saved.</param>
    public DataLookupHolder<T> AddList<T>(string name, bool optional = false) where T : class, IHasId
    {
        var file = CheckCustom<T>(name);
        LoadListHolder(out DataLookupHolder<T> holder, file);
        if (optional && !DataFileExists(file))
            holder.Clear();

        return holder;
    }

    /// <summary>
    /// Adds a dictionary of entities (id to entity) kept in its own data file. The id is the key; an entity that has an id
    /// (<see cref="IHasId"/>) is compared with it when loaded.
    /// </summary>
    /// <param name="optional">The file may be absent: the dictionary starts empty and the file is written when it is saved.</param>
    public DataLookupHolder<T> AddDictionary<T>(string name, bool optional = false) where T : class
    {
        var file = CheckCustom<T>(name);
        LoadDictionaryHolder(out DataLookupHolder<T> holder, file);
        if (optional && !DataFileExists(file))
            holder.Clear();

        return holder;
    }

    /// <summary>Adds a single entity kept in its own data file.</summary>
    /// <param name="optional">The file may be absent: a new entity (its parameterless constructor) is used and the file is written when it is saved.</param>
    public DataHolder<T> AddEntity<T>(string name, bool optional = false) where T : class
    {
        var file = CheckCustom<T>(name);
        LoadHolder(out DataHolder<T> holder, file);
        if (optional && !DataFileExists(file))
            holder.Set((T)Activator.CreateInstance(ConcreteType<T>())!);

        return holder;
    }

    /// <summary>
    /// The names of all holders: the standard ones (<c>Actors</c>, <c>Skills</c>, ..., <c>System</c>, <c>Maps</c>; the name of the data file
    /// without the extension) and the ones added with <see cref="AddList{T}"/>, <see cref="AddDictionary{T}"/> and <see cref="AddEntity{T}"/>.
    /// </summary>
    public IReadOnlyCollection<string> HolderNames => _holders.Keys;

    /// <summary>
    /// A holder by name, e.g. <c>storage.GetHolder&lt;DataLookupHolder&lt;IActor&gt;&gt;("Actors")</c> or
    /// <c>storage.GetHolder&lt;DataLookupHolder&lt;Quest&gt;&gt;("Quests")</c>. The names are not case sensitive.
    /// Throws if there is no such holder or it is of another type.
    /// </summary>
    public THolder GetHolder<THolder>(string name) where THolder : class
    {
        if (!_holders.TryGetValue(name, out var holder))
            throw new KeyNotFoundException($"There is no holder '{name}' (holders: {string.Join(", ", _holders.Keys)})");

        return holder as THolder ??
               throw new InvalidCastException($"The holder '{name}' is {holder.GetType().Name}, not {typeof(THolder).Name}");
    }

    /// <summary>Like <see cref="GetHolder{THolder}"/>, but false when there is no such holder or it is of another type.</summary>
    public bool TryGetHolder<THolder>(string name, out THolder? holder) where THolder : class
    {
        holder = _holders.GetValueOrDefault(name) as THolder;

        return holder != null;
    }

    private string CheckCustom<T>(string name) where T : class
    {
        if (string.IsNullOrWhiteSpace(name) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new ArgumentException($"'{name}' is not a name of a data file", nameof(name));

        CheckCustomType(ConcreteType<T>());

        var file = FileName(name);
        if (_files.Contains(file) || _holders.ContainsKey(name))
            throw new ArgumentException($"The data file {file} is already used by another table", nameof(name));

        return file;
    }

    /// <summary>Throws when the type cannot be kept in a data file of this version without losing members it does not describe.</summary>
    protected virtual void CheckCustomType(Type concrete) =>
        throw new NotSupportedException($"Tables of your own are not supported for {Version} yet");

    private MapsHolder LoadMapsHolderCore<T, TV>()
        where T : class, TV
    {
        var holder = new MapsHolder(MapPath, path => DataFileExists(path) ? (IMap)LoadData<T>(path)! : null, SaveObject);
        _holders["Maps"] = holder;

        return holder;
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
