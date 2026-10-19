using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Exceptions;

namespace RPGMDataKit.Storages.Abstraction;

// The pictures, sounds and movies of the project: the kinds of resource, the folder and the extensions of each, and the files.
//
// A kind is known by its name (not case sensitive). The built-in kinds are the members of ResourceType, with the names of ResourceTypes.Names; the maker says which of them it has and where
// they are (ResourceFolderName). Kinds of your own are added with RegisterResource. Every method that takes a ResourceType has one that takes a name.
public abstract partial class Storage
{
    /// <summary>
    /// The folder of a built-in kind of resource under the project, as the maker names it (<c>Graphics/Tilesets</c> of VX Ace, <c>img/tilesets</c> of MV; with <c>/</c> between the folders), or null if
    /// the maker has no such kind (the sprites of the sideview battle are of MV only).
    /// </summary>
    protected abstract string? ResourceFolderName(ResourceType type);

    private sealed class ResourceKind(string name, string initialFolder, IReadOnlyList<string> extensions)
    {
        public string Name { get; } = name;

        /// <summary>The folder the kind had when it was made (the maker's, or the one given to <c>RegisterResource</c>); <c>ResetResourceFolder</c> goes back to it.</summary>
        public string InitialFolder { get; } = initialFolder;

        public string Folder { get; set; } = initialFolder;

        public IReadOnlyList<string> Extensions { get; } = extensions;

        /// <summary>Made by <c>RegisterResource</c>, so it can be taken away again; the kinds of the maker cannot.</summary>
        public bool IsRegistered { get; init; }
    }

    // made when first asked: the maker's own list is a virtual method, which a constructor must not call
    private OrderedDictionary<string, ResourceKind>? _resourceKinds;

    private OrderedDictionary<string, ResourceKind> ResourceKinds => _resourceKinds ??= BuiltInResourceKinds();

    private OrderedDictionary<string, ResourceKind> BuiltInResourceKinds()
    {
        var kinds = new OrderedDictionary<string, ResourceKind>(StringComparer.OrdinalIgnoreCase);
        foreach (var type in Enum.GetValues<ResourceType>())
        {
            if (ResourceFolderName(type) is { } folder)
                kinds[type.NameOf()] = new ResourceKind(type.NameOf(), folder, BuiltInExtensions(type));
        }

        return kinds;
    }

    // what the maker reads: the pictures of VX Ace are png, jpg and bmp, those of MV png; the sounds of VX Ace are ogg, wav, mp3, wma and midi, those of MV ogg and m4a; the movies of VX Ace are ogv, those of MV webm and mp4
    private IReadOnlyList<string> BuiltInExtensions(ResourceType type)
    {
        var mv = Version == StorageVersion.MV;
        if (type is ResourceType.Bgm or ResourceType.Bgs or ResourceType.Me or ResourceType.Se)
            return mv ? [".ogg", ".m4a"] : [".ogg", ".wav", ".mp3", ".wma", ".mid", ".midi"];
        if (type == ResourceType.Movie)
            return mv ? [".webm", ".mp4"] : [".ogv"];

        return mv ? [".png"] : [".png", ".jpg", ".jpeg", ".bmp"];
    }

    private ResourceKind Kind(string name)
    {
        if (name != null && ResourceKinds.TryGetValue(name.Trim(), out var kind))
            return kind;

        // a name of a built-in kind that this maker does not have is a question of the version, any other name is simply not known
        if (ResourceTypes.TryParse(name, out var type))
            throw new DoesNotExistInVersionException($"{type} does not exist in {Version}");

        throw new ArgumentException($"There is no kind of resource '{name}' (the kinds are: {string.Join(", ", ResourceKinds.Keys)})", nameof(name));
    }

    // ---- the kinds ----

    /// <summary>The names of the kinds of resource of this project: the ones the maker has, in the order of <see cref="ResourceType"/>, then the ones that were registered.</summary>
    public IReadOnlyCollection<string> ResourceNames => ResourceKinds.Keys.ToList();

    /// <summary>Whether the project has a kind of resource of this name (the case does not matter).</summary>
    public bool HasResource(string name) => name != null && ResourceKinds.ContainsKey(name.Trim());

    /// <summary>Whether the project has the built-in kind: the maker has it, or it was registered.</summary>
    public bool HasResourceType(ResourceType type) => HasResource(type.NameOf());

    /// <summary>
    /// Adds a kind of resource of your own (the portraits of a plugin, the pictures of a script): its name, the folder under the project (either separator, or a whole path), and the extensions of the files
    /// (with or without the dot; the first one is looked for first). Without extensions the files are pictures of the maker (png; png, jpg, jpeg, bmp). It is for this storage only. The folder is not checked to exist.
    /// A built-in kind that the maker lacks can be registered by its name too.
    /// </summary>
    /// <exception cref="ArgumentException">The name is empty or has a separator in it, the folder is empty, or the project has the kind already (change its folder with <see cref="SetResourceFolder(string, string)"/>).</exception>
    public void RegisterResource(string name, string folder, IEnumerable<string>? extensions = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        name = name.Trim();
        if (name.Length == 0 || name.IndexOfAny(['/', '\\']) >= 0)
            throw new ArgumentException($"'{name}' is not a name of a kind of resource: it is not empty and has no separators", nameof(name));
        if (ResourceKinds.ContainsKey(name))
            throw new ArgumentException($"The project has the kind of resource '{name}' already", nameof(name));

        var list = extensions?.Select(e => e.Trim()).Where(e => e.Length > 0).Select(e => e.StartsWith('.') ? e : "." + e).ToList();
        ResourceKinds[name] = new ResourceKind(name, CleanFolder(folder), list is { Count: > 0 } ? list : BuiltInExtensions(ResourceType.Picture)) { IsRegistered = true };
    }

    /// <summary>Adds a built-in kind that the maker does not have (the sprites of the sideview battle in VX Ace, when a script of the project adds them). See <see cref="RegisterResource(string, string, IEnumerable{string}?)"/>.</summary>
    public void RegisterResource(ResourceType type, string folder, IEnumerable<string>? extensions = null) =>
        RegisterResource(type.NameOf(), folder, extensions ?? BuiltInExtensions(type));

    /// <summary>Takes away a kind that was added with <see cref="RegisterResource(string, string, IEnumerable{string}?)"/>. The kinds of the maker stay (their folder can be changed and reset).</summary>
    /// <exception cref="ArgumentException">There is no such kind, or it is a kind of the maker.</exception>
    public void UnregisterResource(string name)
    {
        var kind = Kind(name);
        if (!kind.IsRegistered)
            throw new ArgumentException($"'{kind.Name}' is a kind of the maker, not one that was registered: use ResetResourceFolder to give it the folder of the maker back", nameof(name));

        ResourceKinds.Remove(kind.Name);
    }

    // ---- the folders ----

    private static string CleanFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentException("The folder of a kind of resource is not empty", nameof(folder));

        return folder.Replace('\\', '/').Trim();
    }

    /// <summary>
    /// Puts another folder in place of the one the kind has, for this storage only: for a project whose pictures are kept elsewhere (the converter makes the sheets for the game in <c>img/tilesets_game</c>).
    /// The folder is under the project (either separator) or a whole path. It is not checked to exist.
    /// </summary>
    /// <exception cref="ArgumentException">The folder is empty, or there is no such kind.</exception>
    /// <exception cref="DoesNotExistInVersionException">The name is of a built-in kind that the maker lacks (register it first).</exception>
    public void SetResourceFolder(string name, string folder) => Kind(name).Folder = CleanFolder(folder);

    /// <inheritdoc cref="SetResourceFolder(string, string)"/>
    public void SetResourceFolder(ResourceType type, string folder) => SetResourceFolder(type.NameOf(), folder);

    /// <summary>Takes back the folder put by <see cref="SetResourceFolder(string, string)"/>: the kind has the folder it started with again (the maker's, or the one it was registered with).</summary>
    public void ResetResourceFolder(string name)
    {
        var kind = Kind(name);
        kind.Folder = kind.InitialFolder;
    }

    /// <inheritdoc cref="ResetResourceFolder(string)"/>
    public void ResetResourceFolder(ResourceType type) => ResetResourceFolder(type.NameOf());

    /// <summary>The folder of the files of the kind: the folder of the project and the folder of the kind. It is not checked to exist.</summary>
    /// <exception cref="ArgumentException">There is no such kind.</exception>
    /// <exception cref="DoesNotExistInVersionException">The name is of a built-in kind that the maker lacks.</exception>
    public string GetResourceFolder(string name) => Path.Combine(ProjectPath, Kind(name).Folder.Replace('/', Path.DirectorySeparatorChar));

    /// <inheritdoc cref="GetResourceFolder(string)"/>
    public string GetResourceFolder(ResourceType type) => GetResourceFolder(type.NameOf());

    // ---- the files ----

    /// <summary>
    /// The path of a resource: the folder of the kind and the name of the file, as the data names it (<c>Outside_A2</c>, without the extension), and the extension if one is given (<c>".png"</c>). The file need
    /// not exist. A name may have a folder in it (<c>Monsters/Slime</c>), but not lead out of the folder of the kind.
    /// </summary>
    /// <exception cref="ArgumentException">The file name is empty, or is a path that is not under the folder of the kind; or there is no such kind.</exception>
    public string GetResourcePath(string kind, string name, string extension = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The name of a resource is not empty", nameof(name));
        if (Path.IsPathRooted(name) || name.Replace('\\', '/').Split('/').Contains(".."))
            throw new ArgumentException($"'{name}' is not a name under the folder of {kind}", nameof(name));

        return Path.Combine(GetResourceFolder(kind), name.Replace('/', Path.DirectorySeparatorChar) + extension);
    }

    /// <inheritdoc cref="GetResourcePath(string, string, string)"/>
    public string GetResourcePath(ResourceType type, string name, string extension = "") => GetResourcePath(type.NameOf(), name, extension);

    /// <summary>The extensions of the files of the kind, the first one first (see <see cref="RegisterResource(string, string, IEnumerable{string}?)"/> and the table in the docs for the ones of the maker).</summary>
    public IReadOnlyList<string> GetResourceExtensions(string kind) => Kind(kind).Extensions;

    /// <inheritdoc cref="GetResourceExtensions(string)"/>
    public IReadOnlyList<string> GetResourceExtensions(ResourceType type) => GetResourceExtensions(type.NameOf());

    /// <summary>
    /// The file of a resource that exists: the name with one of the extensions of the kind, the case of the name not mattering (a file system that tells the cases apart
    /// would not find <c>outside_a2</c> for <c>Outside_A2</c> otherwise). Null if there is no such file.
    /// </summary>
    public string? FindResourceFile(string kind, string name)
    {
        var extensions = GetResourceExtensions(kind);
        foreach (var extension in extensions)
        {
            var path = GetResourcePath(kind, name, extension);
            if (File.Exists(path))
                return path;
        }

        // the same name in other letters: look through the folder of the file
        var directory = Path.GetDirectoryName(GetResourcePath(kind, name))!;
        if (!Directory.Exists(directory))
            return null;

        var fileName = Path.GetFileName(name.Replace('/', Path.DirectorySeparatorChar));
        var files = Directory.EnumerateFiles(directory).ToList();
        foreach (var extension in extensions)
        {
            var found = files.FirstOrDefault(f => string.Equals(Path.GetFileName(f), fileName + extension, StringComparison.OrdinalIgnoreCase));
            if (found != null)
                return found;
        }

        return null;
    }

    /// <inheritdoc cref="FindResourceFile(string, string)"/>
    public string? FindResourceFile(ResourceType type, string name) => FindResourceFile(type.NameOf(), name);
}
