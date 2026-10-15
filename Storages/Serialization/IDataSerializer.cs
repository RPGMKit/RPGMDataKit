namespace RPGMDataKit.Storages.Serialization;

public interface IDataSerializer
{
    string FileExtension { get; }

    T Load<T>(string path);

    void Save<T>(string path, T data);

    /// <summary>Like <see cref="Load{T}"/>; a serializer that can read the file without blocking overrides it.</summary>
    Task<T> LoadAsync<T>(string path, CancellationToken cancellationToken = default) => Task.FromResult(Load<T>(path));

    /// <summary>Like <see cref="Save{T}"/>; a serializer that can write the file without blocking overrides it.</summary>
    Task SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default)
    {
        Save(path, data);

        return Task.CompletedTask;
    }
}
