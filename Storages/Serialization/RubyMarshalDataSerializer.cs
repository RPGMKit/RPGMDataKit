using RubyMarshalCS;
using RubyMarshalCS.Enums;
using RubyMarshalCS.Settings;

namespace RPGMakerLibCoreV2.Storages.Serialization;

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

    public T Load<T>(string path) => RubyMarshal.Load<T>(path, _settings);

    public void Save<T>(string path, T data) => RubyMarshal.Dump(path, data);
}
