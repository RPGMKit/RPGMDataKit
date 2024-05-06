namespace RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

public interface IByIdSetter<in T>
{
    public void Set(int index, T? value);
}
