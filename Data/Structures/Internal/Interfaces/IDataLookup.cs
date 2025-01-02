namespace RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

public interface IDataLookup<T> : IByIdGetter<T>, IByIdSetter<T>
{
    public IEnumerable<KeyValuePair<int, T>> GetWithIndex();

    public IDataLookup<TV> MorphToBase<TV>() where TV : class;
}
