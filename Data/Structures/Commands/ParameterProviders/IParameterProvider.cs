namespace RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;

public interface IParameterProvider
{
    public T? GetParameter<T>(int index);

    public void SetParameter<T>(int index, T? value);
    
    public int Count { get; }
    
    public void Clear();
}