namespace RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

public interface IToTextSerializer
{
    public string Serialize<T>(T obj);

    public T Deserialize<T>(string text);
}
