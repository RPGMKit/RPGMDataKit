using Newtonsoft.Json;

namespace RPGMDataKit.Storages.Serialization;

public class JsonDataSerializer : IDataSerializer
{
    private readonly JsonSerializerSettings _settings;

    public JsonDataSerializer(TypeMap types)
    {
        _settings = types.CreateJsonSettings();
    }

    public string FileExtension => ".json";

    public T Load<T>(string path) => Parse<T>(path, File.ReadAllText(path));

    public async Task<T> LoadAsync<T>(string path, CancellationToken cancellationToken = default) =>
        Parse<T>(path, await File.ReadAllTextAsync(path, cancellationToken));

    public void Save<T>(string path, T data)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(data, _settings));
    }

    public Task SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default) =>
        File.WriteAllTextAsync(path, JsonConvert.SerializeObject(data, _settings), cancellationToken);

    private T Parse<T>(string path, string contents)
    {
        if (string.IsNullOrWhiteSpace(contents))
            throw new FileLoadException(path);

        return JsonConvert.DeserializeObject<T>(contents, _settings)!;
    }
}
