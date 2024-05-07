namespace RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

public interface IByIdGetter<out T> : IEnumerable<T>
{
    public bool HasIndex(int index);

    public T? Get(int index);
}
