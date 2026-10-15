using RubyMarshalCS;
using RubyMarshalCS.Enums;
using RubyMarshalCS.Settings;

namespace RPGMDataKit.Storages.Serialization;

public class RubyMarshalDataSerializer : IDataSerializer
{
    /// <summary>Where the notes about the unknown members and objects of a file go; the console by default, <c>null</c> keeps quiet.</summary>
    public static Action<string>? Log { get; set; } = null;

    private readonly DeserializationSettings _settings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Error,
        UndefinedObjectHandling = UndefinedObjectHandling.Error,
        UndefinedUserObjectHandling = UndefinedUserObjectHandling.Error,

        MissingMember = (_, args) =>
        {
            Log?.Invoke($"Missing member \"{args.Member.Key}\" found for object \"{args.RubyObject.GetRealClassName()}\"");
            args.Store();
        },
        UndefinedObject = (_, args) =>
        {
            Log?.Invoke($"Undefined object found: \"{args.RubyObject.GetRealClassName()}\"");
            args.Ignore();
        },
        UndefinedUserObject = (_, args) =>
        {
            Log?.Invoke($"Undefined user object found: \"{args.RubyUserObject.GetRealClassName()}\"");
            args.Store();
        }
    };

    public string FileExtension => ".rvdata2";

    // RubyMarshalCS keeps its caches of the types in static dictionaries that are not safe for several threads,
    // so reading and writing the Ruby objects goes one at a time; the files themselves are read and written without blocking.
    private static readonly object Gate = new();

    public T Load<T>(string path)
    {
        lock (Gate)
            return RubyMarshal.Load<T>(path, _settings)!;
    }

    public async Task<T> LoadAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        var bytes = await File.ReadAllBytesAsync(path, cancellationToken);

        lock (Gate)
            return RubyMarshal.Load<T>(bytes, _settings)!;
    }

    public void Save<T>(string path, T data)
    {
        lock (Gate)
            RubyMarshal.Dump(path, data);
    }

    public async Task SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default)
    {
        byte[] bytes;
        lock (Gate)
            bytes = RubyMarshal.Dump(data);

        await File.WriteAllBytesAsync(path, bytes, cancellationToken);
    }
}
