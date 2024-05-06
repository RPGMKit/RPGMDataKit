namespace RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

public interface IDataLookup<T> : IByIdGetter<T>, IByIdSetter<T>, IEnumerable<T>
{
}
