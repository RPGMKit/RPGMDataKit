namespace RPGMakerLibCoreV2.Storages.Serialization;

public interface IDataSerializer
{
    string FileExtension { get; }

    T Load<T>(string path);

    void Save<T>(string path, T data);
}
