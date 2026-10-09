using Newtonsoft.Json;

namespace RPGMakerLibCoreV2.Storages.Serialization;

public class JsonDataSerializer : IDataSerializer
{
    public string FileExtension => ".json";

    public T Load<T>(string path)
    {
        var contents = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(contents))
            throw new FileLoadException(path);

        return JsonConvert.DeserializeObject<T>(contents)!;
    }

    public void Save<T>(string path, T data)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(data));
    }
}
