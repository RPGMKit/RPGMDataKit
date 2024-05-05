namespace RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

public interface IDataLookup<T>
{
    public bool HasIndex(int index);

    public T Get(int index);

    public void Set(int index, T value);
}
